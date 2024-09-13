using BookStore.FrontEnd.Site.Models.Dtos;
using BookStore.FrontEnd.Site.Models.Infra;
using BookStore.FrontEnd.Site.Models.interfaces;
using BookStore.FrontEnd.Site.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.FrontEnd.Site.Models.Services
{
    public class MemberService
    {
        private IMemberRepository _repo;

        public MemberService()
        {
            _repo = new MemberRepository();
        }

        public MemberService(IMemberRepository repo)
        {
            _repo=repo;
        }

        public void Register(RegisterDto dto)
        {
            // 判斷是否帳號已存在
            bool isAccountExist = _repo.IsAccountExist(dto.Account);
            if (isAccountExist) throw new Exception("帳號已存在");
            // 建立新會員
            // 為密碼進行雜湊
            string salt = HashUtility.GetSalt();
            string hasPassword = HashUtility.ToSHA256(dto.Password, salt);
            // 加入 confirm code
            string confirmCode = Guid.NewGuid().ToString("N");
            // 建立新會員
            dto.ConfirmCode = confirmCode;
            dto.EncryptedPassword = hasPassword;
            dto.IsConfirmed = false;
            _repo.Create(dto);
            // todo 寄送驗證信

        }

        internal void ActiveRegister(int memberId, string confirmCode)
        {
            // 判斷 memberId 和 confirmCode 是否正確，正確就將會員狀態改為已啟用
            MemberDto dto = _repo.Get(memberId);
            if (dto== null) throw new Exception("會員不存在");
            if (dto.ConfirmCode != confirmCode) throw new Exception("驗證碼錯誤");
            if (dto.IsConfirmed.HasValue &&
                dto.IsConfirmed.Value) 
            {
                throw new Exception("會員已啟用"); 
            }

            // 啟用會員
            _repo.Active(memberId);

        }

        internal Result ValidateLogin(LoginDto dto)
        {
            // 找出 User
            MemberDto member = _repo.Get(dto.Account);
            if (member == null)
                return Result.Fail("帳號或密碼錯誤"); // 會員不存在

            // 是否已啟用
            if (!member.IsConfirmed.HasValue || member.IsConfirmed.Value == false)
            {
                return Result.Fail("帳號尚未啟用");
            }

            // 將密碼雜湊後比對
            string hashPassword = HashUtility.ToSHA256(dto.Password);
            bool isPasswordCorrect = (hashPassword.CompareTo(member.EncryptedPassword) == 0);

            // 回傳結果
            return isPasswordCorrect
                ? Result.Success()
                : Result.Fail("帳號或密碼錯誤"); // 密碼錯誤

        }

        internal void UpdateProfile(EditProfileDto dto)
        {
            //update name,email,mobile欄位值
            //直接叫用EF，或者仍然叫用 IMemberRepository?
            MemberDto memberInDb = _repo.Get(dto.Id);
            memberInDb.Name = dto.Name;
            memberInDb.Email= dto.Email;
            memberInDb.Mobile= dto.Mobile;

            _repo.Update(memberInDb);
        }
    }
}
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
    }
}
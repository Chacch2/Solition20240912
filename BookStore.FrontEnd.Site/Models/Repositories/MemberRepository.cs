using BookStore.FrontEnd.Site.Models.Dtos;
using BookStore.FrontEnd.Site.Models.EFModels;
using BookStore.FrontEnd.Site.Models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace BookStore.FrontEnd.Site.Models.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private AppDbContext _db;
        public MemberRepository()
        {
            _db = new AppDbContext();
            
        }

        public MemberRepository(AppDbContext db) 
        {
            _db = db;
        }
        public void Create(RegisterDto dto)
        {
            _db.Members.Add(new Member
            {
                Account = dto.Account,
                Email = dto.Email,
                EncryptedPassword = dto.EncryptedPassword,
                Name = dto.Name,
                Mobile = dto.Mobile,
                ConfirmCode = dto.ConfirmCode,
                IsConfirmed = dto.IsConfirmed
            });

            _db.SaveChanges();

        }
        public MemberDto Get(int memberId)
        {
            var member = _db.Members.FirstOrDefault(x => x.Id == memberId);
            if (member == null) return null;

            return new MemberDto
            {
                Id = member.Id,
                Account = member.Account,
                Email = member.Email,
                EncryptedPassword = member.EncryptedPassword,
                Name = member.Name,
                Mobile = member.Mobile,
                ConfirmCode = member.ConfirmCode,
                IsConfirmed = member.IsConfirmed
            };
        }

        public void Active(int memberId)
        {
            var member = _db.Members.FirstOrDefault(x => x.Id == memberId);
            member.IsConfirmed = true;
            member.ConfirmCode = null;

            _db.SaveChanges();
        }

        public bool IsAccountExist(string account)
        {
            var member = _db.Members.FirstOrDefault(x => x.Account == account);

            return member != null;
        }

        public MemberDto Get(string account)
        {
            var member = _db.Members.FirstOrDefault(x => x.Account == account);
            if (member == null) return null;

            return WebApiApplication._mapper.Map<MemberDto>(member);
        }
    }
}
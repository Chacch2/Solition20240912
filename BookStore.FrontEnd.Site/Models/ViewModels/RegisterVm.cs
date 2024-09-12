using BookStore.FrontEnd.Site.Models.Dtos;
using BookStore.FrontEnd.Site.Models.Infra;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Member = BookStore.FrontEnd.Site.Models.EFModels.Member;

namespace BookStore.FrontEnd.Site.Models.ViewModels
{
    public class RegisterVm
    {
        public int Id { get; set; }

        [Display(Name = "帳號")]
        [Required]
        [StringLength(30)]
        public string Account { get; set; }

        // <summary>
        // 密碼
        // </summary>
        [Display(Name = "密碼")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]
        [Required]
        [StringLength(50)]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }


        [Display(Name = "姓名")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }


        [Display(Name = "手機")]
        [StringLength(10)]
        public string Mobile { get; set; }


    }

    public static class RegisterExt
    {
        public static Member ToMember(this RegisterVm vm)
        {
            var salt = HashUtility.GetSalt();
            var hashPassword = HashUtility.ToSHA256(vm.Password, salt); //建立密碼的雜湊值
            var confirmCode = Guid.NewGuid().ToString("N"); // 發送確認信時使用

            return new Member
            {
                Id = vm.Id, //這行可有可無,用不到
                Account = vm.Account,
                EncryptedPassword = hashPassword,
                Email = vm.Email,
                Name = vm.Name,
                Mobile = vm.Mobile,
                IsConfirmed = false, //新會員預設狀態是:未確認
                ConfirmCode = confirmCode
            };
        }

        public static RegisterDto ToDto(this RegisterVm vm)
        {
            return new RegisterDto
            {
                Id = vm.Id,
                Account = vm.Account,
                Password = vm.Password,
                Email = vm.Email,
                Name = vm.Name,
                Mobile = vm.Mobile
            };
        }

    }
}

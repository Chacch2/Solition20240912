using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.FrontEnd.Site.Models.ViewModels
{
    public class ChangePasswordVm
    {
        [Display(Name = "原始密碼")]
        [Required]
        [DataType(DataType.Password)]
        public string OriginPassword { get; set; }

        [Display(Name = "新密碼")]
        [Required]
        [DataType(DataType.Password)]
        public string ChangePassword {  get; set; }

        [Display(Name = "確認新密碼")]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.FrontEnd.Site.Models.ViewModels
{
    public class CheckoutVm
    {
        [Display(Name = "收件人")]
        [Required]
        [StringLength(30)]
        public string Receiver { get; set; }

        [Display(Name = "地址")]
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Display(Name = "手機")]
        [Required]
        [StringLength(10)]
        public string CellPhone { get; set; }
    }
}
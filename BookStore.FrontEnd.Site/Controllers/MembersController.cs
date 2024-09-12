using BookStore.FrontEnd.Site.Models;
using BookStore.FrontEnd.Site.Models.Dtos;
using BookStore.FrontEnd.Site.Models.Services;
using BookStore.FrontEnd.Site.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace BookStore.FrontEnd.Site.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            Result result = HandleRegister(vm); // 呼叫副程式進行建立新會員的工作，並回傳結果 (成功或失敗，失敗訊息)

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            return View("RegisterConfirm"); // 顯示 RegisterConfirm 頁面內容, 不必有 action
                                            // return RedirectToAction("RegisterConfirm"); // 如果想要轉到某個 action, 就這麼寫
        }


        private Result HandleRegister(RegisterVm vm) 
        {
            // 在這裡，可以自行決定要叫用EF or Service object進行create member的工作
            MemberService service = new MemberService();

            try
            {
                RegisterDto dto = vm.ToDto();
                service.Register(dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

    }
}
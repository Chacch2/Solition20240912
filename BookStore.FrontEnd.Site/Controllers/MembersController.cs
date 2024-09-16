using BookStore.FrontEnd.Site.Models;
using BookStore.FrontEnd.Site.Models.Dtos;
using BookStore.FrontEnd.Site.Models.EFModels;
using BookStore.FrontEnd.Site.Models.Infra;
using BookStore.FrontEnd.Site.Models.Repositories;
using BookStore.FrontEnd.Site.Models.Services;
using BookStore.FrontEnd.Site.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace BookStore.FrontEnd.Site.Controllers
{
    public class MembersController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

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


        public ActionResult ActiveRegister(int memberId, string confirmCode)
        {
            Result result = HandleActiveRegister(memberId, confirmCode);

            //if (!result.IsSuccess)
            //{
            //    ModelState.AddModelError(string.Empty, result.ErrorMessage);
            //    return View();
            //}

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginVm vm)
        {
            if (ModelState.IsValid)
            {
                Result result = HandleLogin(vm);
                if (result.IsSuccess)
                {
                    (string url, HttpCookie cookie) = ProcessLogin(vm.Account);

                    Response.Cookies.Add(cookie);
                    return Redirect(url);
                }
                ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage);


            }
            return View();

        }

        [Authorize]
        public ActionResult EditProfile()
        {
            var account = User.Identity.Name;
            MemberDto dto = new MemberRepository().Get(account);

            EditProfileVm vm = WebApiApplication._mapper.Map<EditProfileVm>(dto);
            return View(vm);

        }

        [Authorize]
        [HttpPost]
        public ActionResult EditProfile(EditProfileVm vm)
        {
            string account = User.Identity.Name;
            Result result = HandleUpdateProfile(account, vm);

            if (result.IsSuccess)
            {
                return RedirectToAction("Index"); // 更新成功，回會員中心頁
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);

        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordVm vm)
        {
            string account = User.Identity.Name;
            Result result = HandleChangePassword(account, vm);

            if (result.IsSuccess)
            {
                return RedirectToAction("Index"); // 更新成功，回會員中心頁
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(ForgetPasswordVm vm)
        {
            if (ModelState.IsValid == false)
                return View(vm);

            var urlTemplate = Request.Url.Scheme + "://" + //
                              Request.Url.Authority +"/"+ //
                              "/Members/ResetPassword?memberId={0}&confirmCode={1}";//
            
            var result = ProcessResetPassword(vm.Account, vm.Email, urlTemplate);

            if (result.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            return View("ForgetPasswordConfirm");

        }

        public ActionResult ResetPassword(int memberId, string confirmCode)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVm vm ,int memberId,string confirmCode)
        {
            if (ModelState.IsValid == false)
                return View(vm);

            Result result = ProcessChangePassword(memberId, confirmCode, vm.Password);

            if (result.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            return View("ResetPasswordConfirm");
        }

        private Result ProcessChangePassword(int memberId, string confirmCode, string password)
        {
            var db = new AppDbContext();

            // 檢查memberId, confirmCode是否正確
            var memberInDb = db.Members.FirstOrDefault(m => m.Id == memberId && m.ConfirmCode == confirmCode);

            if (memberInDb == null)
                return Result.Fail("找不到對應的會員紀錄");

            var salt = HashUtility.GetSalt();
            var encryptedPassword = HashUtility.ToSHA256(password, salt);

            memberInDb.EncryptedPassword = encryptedPassword;
            memberInDb.ConfirmCode = null;
            
            db.SaveChanges();


            return Result.Success();
        }

        private Result ProcessResetPassword(string account, string email, string urlTemplate)
        {
            var db = new AppDbContext();

            // 檢查account, email是否正確
            var memberInDb = db.Members.FirstOrDefault(m => m.Account == account);

            if (memberInDb == null)
                return Result.Fail("帳號或 Email 錯誤");  // 故意不告知確切錯誤原因

            if (string.Compare(email, memberInDb.Email, StringComparison.CurrentCultureIgnoreCase) != 0)
                return Result.Fail("帳號或 Email 錯誤");  

            // 檢查 IsConfirmed 必須為 true，因為只有已啟用的帳號才能重設密碼
            if (memberInDb.IsConfirmed == false)
                return Result.Fail("帳號未啟用，請先完成啟用程序");

            // 更新記錄，填入 confirmCode
            var confirmCode = Guid.NewGuid().ToString("N");
            memberInDb.ConfirmCode = confirmCode;
            db.SaveChanges();

            // 發送 email
            var url = string.Format(urlTemplate, memberInDb.Id, confirmCode);
            new EmailHelper().SendForgotPasswordEmail(url, memberInDb.Name, email);

            return Result.Success();

        }

        private Result HandleChangePassword(string account, ChangePasswordVm vm)
        {
            var service = new MemberService();
            try
            {
                ChangePasswordDto dto = WebApiApplication._mapper.Map<ChangePasswordDto>(vm);
                service.UpdatePassword(account,dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        private Result HandleUpdateProfile(string account, EditProfileVm vm)
        {
            var service = new MemberService();
            try
            {
                EditProfileDto dto = WebApiApplication._mapper.Map<EditProfileDto>(vm);
                service.UpdateProfile(dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        private (string url, HttpCookie cookie) ProcessLogin(string account)
        {
            var roles = string.Empty; // 角色欄位，沒有用到角色權限，所以存入空白

            // 建立一張認證憑證
            var ticket = new FormsAuthenticationTicket(
                1,                              // 版本別，沒有特別用途
                account,                        // 使用者帳號
                DateTime.Now,                   // 發行日
                DateTime.Now.AddDays(2),        // 到期日
                false,                          // 是否記住
                roles,                          // 使用者資料
                "/"                             // cookie 位置
            );

            // 將它加密
            var value = FormsAuthentication.Encrypt(ticket);

            // 存入 cookie
            var cookies = new HttpCookie(FormsAuthentication.FormsCookieName, value);

            // 取得 return url
            var url = FormsAuthentication.GetRedirectUrl(account, true); // 第二個引數沒有用途

            return (url, cookies);

        }

        private Result HandleLogin(LoginVm vm)
        {
            try
            {
                var service = new MemberService();

                LoginDto dto = WebApiApplication._mapper.Map<LoginDto>(vm); //automapper

                Result validateResult = service.ValidateLogin(dto);

                return validateResult;

            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Members");
        }
        private Result HandleActiveRegister(int memberId, string confirmCode)
        {
            try
            {
                var service = new MemberService();
                service.ActiveRegister(memberId, confirmCode);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
            

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
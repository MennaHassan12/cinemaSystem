using cinemaSystem.Interfaces;
using cinemaSystem.Models;
using cinemaSystem.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace cinemaSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _emailSender = emailSender;


        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailORUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailORUserName);

            if (user is null)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
                ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

                return View(loginVM);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Confirm Your Email First");

                return View(loginVM);
            }

            //    OLD WAY    //
            //var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

            //if (!result)
            //{
            //    ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Invalid User Name Or Email");
            //    ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            //    return View(loginVM);
            //}
            //if (!user.EmailConfirmed)
            //{
            //    ModelState.AddModelError(nameof(LoginVM.EmailORUserName), "Confirm Your Email First");

            //    return View(loginVM);
            //}
            TempData["success_notification"] = $"Welcome Back {user.FirstName} {user.LastName}";
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser user = new()
            {
                FirstName = registerVM.FName,
                LastName = registerVM.LName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError(string.Empty, item.Description);

                return View(registerVM);
            }
            var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), "Account", new { area = "Identity" }
            , Request.Scheme
            );
            await _emailSender.SendEmailAsync(user.Email, "Confirmation Your Account in Cinema App", $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>");



            TempData["success_notification"] = "Add Account Successfully, check you email";

            return RedirectToAction("Login");


        }

        public async Task<IActionResult> Confirm(string token, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                TempData["error_notification"] = String.Join(",", result.Errors.Select(e => e.Description));

            TempData["success_notification"] = "Confirm Email Successfully, You can login now";

            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailORUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailORUserName);

            var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), "Account", new { area = "Identity" }
            , Request.Scheme
            );
            await _emailSender.SendEmailAsync(user.Email!, "Confirmation Your Account in Cinema App", $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>");


            TempData["success_notification"] = $"Resend Email Confirmation successfully, please check yoy email";

            return RedirectToAction(nameof(Login));
        }
       
        //public IActionResult ForgetPassword()
        //{
        //    return View();
        //}
        //public IActionResult ResetPassword()
        //{
        //    return View();
        //}

        //public IActionResult ValidOTP()
        //{
        //    return View();
        //}

    }


}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTestWebApp.Models;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Threading.Tasks;

namespace MyTestWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager) 
        { 
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ViewRegisterModel model)
        {
            //recaptcha
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ModelState.AddModelError("", "Captcha answer cannot be empty.");
                return View(model);
            }
            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
            if (!recaptchaResult.Success)
            {
                ModelState.AddModelError("", "Incorrect captcha answer.");
            }

            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName, Admin = model.IsAdmin };
                var result = await _userManager.CreateAsync(user, model.Password);

                string role = model.IsAdmin ? "admin" : "user";
                var result2 = await _userManager.AddToRoleAsync(user, role);

                if (result.Succeeded && result2.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    foreach (var error in result2.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new ViewLoginModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ViewLoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Ads");
                    }
                }
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Account");
        }
    }
}

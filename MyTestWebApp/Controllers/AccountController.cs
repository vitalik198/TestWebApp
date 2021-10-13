using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTestWebApp.Models;
using System;
using System.Threading.Tasks;
using Recaptcha.Web.Mvc;
using Recaptcha.Web;

namespace MyTestWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager; 

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

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
                User user = new User { Email = model.Email, UserName = model.UserName, Admin = model.IsAdmin};
                var result = await userManager.CreateAsync(user, model.Password);

                //Roles Seed {
                if (await roleManager.FindByNameAsync("user") == null)
                    await roleManager.CreateAsync(new IdentityRole("user"));
                if (await roleManager.FindByNameAsync("admin") == null)
                    await roleManager.CreateAsync(new IdentityRole("admin"));
                //Roles Seed }

                string role = model.IsAdmin ? "admin" : "user";
                var result2 = await userManager.AddToRoleAsync(user, role);

                if (result.Succeeded && result2.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
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
        public IActionResult Login(string returnUrl)
        {
            return View(new ViewLoginModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ViewLoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    await signInManager.SignOutAsync();

                    var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(model.ReturnUrl ?? "/");
                    }                    
                }
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
           await signInManager.SignOutAsync();

           return RedirectToAction("Index", "Account");
        }     
    }
}

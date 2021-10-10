using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTestWebApp.Models;
using System.Threading.Tasks;

namespace MyTestWebApp.Controllers
{
    /// <summary>
    /// API controllers for login and logout actions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ApiAccountController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Action method for log in acount
        /// </summary>
        /// <param name="loginModel">Log in data: userName and password</param>
        /// <returns>return 200 status code if succeeded</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ViewLoginApiModel loginModel)
        {
            User user = await userManager.FindByNameAsync(loginModel.UserName);
            var result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (result.Succeeded)
                return Ok();
            else
                return Unauthorized();
        }
        /// <summary>
        /// Action method for log out
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ViewRegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = registerModel.Email, UserName = registerModel.UserName, Admin = registerModel.IsAdmin };
                var result = await userManager.CreateAsync(user, registerModel.Password);

                //Roles Seed
                if (await roleManager.FindByNameAsync("user") == null)
                    await roleManager.CreateAsync(new IdentityRole("user"));
                if (await roleManager.FindByNameAsync("admin") == null)
                    await roleManager.CreateAsync(new IdentityRole("admin"));
                //Roles Seed

                string role = registerModel.IsAdmin ? "admin" : "user";
                var result2 = await userManager.AddToRoleAsync(user, role);

                if (result.Succeeded && result2.Succeeded)
                {
                    return Ok();
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

            return BadRequest(ModelState);
        }
    }
}

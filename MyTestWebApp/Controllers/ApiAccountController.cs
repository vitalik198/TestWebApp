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
    }
}

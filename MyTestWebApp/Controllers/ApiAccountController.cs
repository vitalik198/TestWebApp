using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTestWebApp.Models;
using System.Threading.Tasks;

namespace MyTestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<User> roleManager;

        public ApiAccountController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<User> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ViewLoginModel loginModel)
        {
            User user= await userManager.FindByNameAsync(loginModel.UserName);
            var result = await signInManager.PasswordSignInAsync(user, loginModel.Password,false,false);
            if (result.Succeeded)
                return Ok();
            else 
               return Unauthorized();
        }
    }
}

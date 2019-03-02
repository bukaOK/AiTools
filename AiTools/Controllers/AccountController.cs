using AiTools.BLL.Services.Interfaces;
using AiTools.DAL.Managers;
using AiTools.Models.AccountModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly SignInManager signInManager;

        public AccountController(IUserService userService, SignInManager signInManager)
        {
            this.userService = userService;
            this.signInManager = signInManager;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await userService.RegisterAsync(model);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err);
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {   
            if (ModelState.IsValid)
            {
                var result = await userService.LoginAsync(model);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                }
            }
            return BadRequest(ModelState);
        }
        public async Task<IActionResult> LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                await signInManager.SignOutAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}

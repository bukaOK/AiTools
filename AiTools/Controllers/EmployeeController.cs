using AiTools.BLL.Services.Interfaces;
using AiTools.CodeData.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class EmployeeController : BaseController
    {
        private readonly IUserService userService;

        public EmployeeController(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<IActionResult> GetInviteKey()
        {
            var key = await userService.GetInviteKeyAsync(UserId);
            return Ok(key);
        }
        public async Task<IActionResult> GetAll()
        {
            var result = await userService.GetEmployeesAsync();
            return Ok(result);
        }
        public async Task<IActionResult> CreateInviteKey()
        {
            var result = await userService.CreateInviteKeyAsync(UserId);
            return Ok(result.ResultData);
        }
    }
}

using AiTools.DAL.Managers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager userManager;

        public HomeController(UserManager userManager)
        {
            this.userManager = userManager;
        }
        //public async Task<IActionResult> ClientState()
        //{
        //}
    }
}

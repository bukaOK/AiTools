using AiTools.BLL.Services.Interfaces;
using AiTools.Models.AnalyzeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    [Authorize]
    public class AnalyzeController : BaseController
    {
        private readonly IAnalyzeService analyzeService;

        public AnalyzeController(IAnalyzeService analyzeService)
        {
            this.analyzeService = analyzeService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(EditAnalyzeModel model)
        {
            model.UserId = UserId;
            var result = await analyzeService.AddAsync(model);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500);
        }
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var result = await analyzeService.GetSavedAsync(UserId);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetData(Guid id)
        {
            var data = await analyzeService.GetDataAsync(id, UserId);
            return Ok(data);
        }
        public async Task<IActionResult> RemoveResult(Guid id)
        {
            var result = await analyzeService.RemoveAsync(id);
            if (result.Succeeded)
                return Ok();
            return StatusCode(500);
        }
    }
}

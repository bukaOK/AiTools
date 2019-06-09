using AiTools.BLL.Services.Interfaces;
using AiTools.Models.ForecastModels;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    [Authorize]
    public class ForecastController : BaseController
    {
        private readonly IForecastService forecastService;

        public ForecastController(IForecastService forecastService)
        {
            this.forecastService = forecastService;
        }
        [HttpGet]
        public async Task<IActionResult> GetForEdit(Guid id)
        {
            var model = await forecastService.GetForEditAsync(id);
            return Ok(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var result = await forecastService.GetByUserAsync(UserId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Train(ForecastEditModel model)
        {
            if(model.File == null || model.File.Length == 0)
                ModelState.AddModelError(nameof(model.File), "Файл не выбран");
            if (string.IsNullOrEmpty(model.Delimiter))
                ModelState.AddModelError(nameof(model.Delimiter), "Заполните разделитель");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.UserId = UserId;
            var result = await forecastService.TrainAsync(model);
            if(result.Succeeded)
                return Ok(result.ResultData);
            ModelState.AddModelError("", result.Errors.First());
            return StatusCode(500, ModelState);
        }
        [HttpPost]
        public async Task<IActionResult> Predict(ForecastEditModel model)
        {
            if (model.File == null || model.File.Length == 0)
                ModelState.AddModelError(nameof(model.File), "Файл не выбран");
            if (string.IsNullOrEmpty(model.Delimiter))
                ModelState.AddModelError(nameof(model.Delimiter), "Заполните разделитель");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.UserId = UserId;
            var result = await forecastService.PredictAsync(model);
            if (result.Succeeded)
                return Ok(result.ResultData);
            ModelState.AddModelError("", result.Errors.First());
            return StatusCode(500, ModelState);
        }
    }
}

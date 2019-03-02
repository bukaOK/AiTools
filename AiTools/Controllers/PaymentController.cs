using AiTools.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Controllers
{
    public class PaymentController : Controller
    {
        public async Task<IActionResult> YandexPayIn(SumPayModel model)
        {
            if (model.Sum == 0)
                ModelState.AddModelError(nameof(model.Sum), "Сумма должна быть больше 0");
            if (!ModelState.IsValid)
                return BadRequest(model);
            return Ok();
        }
    }
}

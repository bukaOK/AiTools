using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.Models.ForecastModels
{
    public class ForecastEditModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Delimiter { get; set; }
        public IFormFile File { get; set; }
    }
}

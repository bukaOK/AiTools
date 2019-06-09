using AiTools.BLL.Infrastructure;
using AiTools.Models.ForecastModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AiTools.BLL.Services.Interfaces
{
    public interface IForecastService
    {
        Task<ForecastEditModel> GetForEditAsync(Guid id);
        Task<IList<SelectListItem>> GetByUserAsync(string userId);
        Task<DataServiceResult> TrainAsync(ForecastEditModel model);
        Task<DataServiceResult> PredictAsync(ForecastEditModel model);
    }
}

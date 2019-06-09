using AiTools.BLL.Infrastructure;
using AiTools.Models.AnalyzeModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AiTools.BLL.Services.Interfaces
{
    public interface IAnalyzeService
    {
        Task<EditAnalyzeModel> GetDataAsync(Guid id, string userId);
        Task<IList<SelectListItem>> GetSavedAsync(string userId);
        Task<DataServiceResult> AddAsync(EditAnalyzeModel model);
        Task<DataServiceResult> UpdateAsync(EditAnalyzeModel model);
        Task<DataServiceResult> RemoveAsync(Guid id);
    }
}

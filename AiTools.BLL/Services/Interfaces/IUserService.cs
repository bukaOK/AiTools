using AiTools.BLL.Infrastructure;
using AiTools.Models.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.BLL.Services.Interfaces
{
    public interface IUserService : IDataService
    {
        Task<DataServiceResult> RegisterAsync(RegisterModel registerModel);
        Task<DataServiceResult> LoginAsync(LoginModel loginModel);
    }
}

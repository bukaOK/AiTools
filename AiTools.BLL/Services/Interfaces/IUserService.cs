using AiTools.BLL.Infrastructure;
using AiTools.Models.AccountModels;
using AiTools.Models.EmployeeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.BLL.Services.Interfaces
{
    public interface IUserService : IDataService
    {
        Task<string> GetInviteKeyAsync(string userId);
        Task<DataServiceResult> CreateInviteKeyAsync(string userId);
        Task<IList<EmployeeModel>> GetEmployeesAsync();
        Task<DataServiceResult> RegisterAsync(RegisterModel registerModel);
        Task<DataServiceResult> LoginAsync(LoginModel loginModel);
    }
}

using AiTools.BLL.Infrastructure;
using AiTools.BLL.Services.Interfaces;
using AiTools.CodeData.Constants;
using AiTools.DAL.Entities;
using AiTools.DAL.Managers;
using AiTools.Models.AccountModels;
using AiTools.Models.EmployeeModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.BLL.Services
{
    public class UserService : DataService, IUserService
    {
        private readonly IMapper mapper;
        private readonly SignInManager signInManager;
        private readonly UserManager userManager;

        public UserService(ILogger<UserService> logger, IMapper mapper, SignInManager signInManager, UserManager userManager) : base(logger)
        {
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<DataServiceResult> CreateInviteKeyAsync(string userId)
        {
            try
            {
                var inviteKey = Guid.NewGuid().ToString("N");
                var user = await userManager.FindByIdAsync(userId);
                user.InviteKey = inviteKey;
                await userManager.UpdateAsync(user);

                return DataServiceResult.Success(inviteKey);
            }
            catch(Exception e)
            {
                return CommonError("Ошибка при создании ключа приглашения", e);
            }
        }

        public async Task<IList<EmployeeModel>> GetEmployeesAsync()
        {
            var users = await userManager.Users.ToListAsync();
            return mapper.Map<IList<EmployeeModel>>(users);
        }

        public async Task<string> GetInviteKeyAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user.InviteKey;
        }

        public async Task<DataServiceResult> LoginAsync(LoginModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (result.Succeeded)
                return DataServiceResult.Success();
            else
            {
                var error = "Неверный логин/пароль";
                if (result.IsNotAllowed)
                    error = "Вход запрещен";
                else if (result.IsLockedOut)
                    error = "Вы заблокированы, попробуйте позже";
                else if (result.RequiresTwoFactor)
                    error = "Требуется двухфакторная авторизация";
                return DataServiceResult.Failed(error);
            }
        }

        public async Task<DataServiceResult> RegisterAsync(RegisterModel model)
        {
            var users = await userManager.GetUsersInRoleAsync(RoleNames.Admin);
            if (!users.Any(x => x.InviteKey == model.InviteKey))
                return DataServiceResult.Failed("Неверный ключ приглашения");
            var user = mapper.Map<User>(model);

            var exUser = await userManager.FindByEmailAsync(model.Email);
            if (exUser != null)
                return DataServiceResult.Failed("Пользователь уже существует");

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return DataServiceResult.Success();
            }
            else
            {
                return DataServiceResult.Failed(result.Errors);
            }
        }
    }
}

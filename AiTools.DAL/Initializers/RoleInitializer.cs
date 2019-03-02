using AiTools.DAL.Entities;
using AiTools.DAL.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AiTools.DAL.Initializers
{
    public class RoleInitializer : IInitializer
    {
        private readonly UserManager userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleInitializer(UserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            //Создаем роли
            var roles = new[] { "admin", "client" };
            var existingRoles = await roleManager.Roles.ToListAsync();
            if(existingRoles.Count == 0)
            {
                foreach (var role in roles)
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            //Создаем админа(первый юзер)
            var adminRole = await roleManager.FindByNameAsync("admin");

            var adminEmail = "ponchitos16@gmail.com";
            var adminPass = "qwerty123";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if(admin == null)
            {
                admin = new User
                {
                    Email = adminEmail,
                    FirstName = "Раиль",
                    SirName = "Файрушин",
                    UserName = adminEmail
                };
                await userManager.CreateAsync(admin, adminPass);
                await userManager.AddToRolesAsync(admin, roles);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using AiTools.DAL.Initializers;
using AiTools.DAL.Managers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiTools
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    IInitializer[] initalizers = new[]
                    {
                        new RoleInitializer(userManager, roleManager)
                    };
                    foreach (var init in initalizers)
                        await init.InitializeAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            await host.RunAsync();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureLogging((context, logging) =>
            {
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddEventSourceLogger();
            }).UseStartup<Startup>().Build();
    }
}

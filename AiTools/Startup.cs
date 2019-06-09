using AiTools.DAL.Entities;
using AiTools.DAL.Infrastucture;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;
using AiTools.DAL.Managers;
using AiTools.Models.HandlerModels;
using AiTools.BLL.Infrastructure;
using System.Linq;
using System;
using AiTools.BLL.Providers;
using AiTools.DAL.Core;
using System.Collections.Generic;

namespace AiTools
{
    public class Startup
    {
        private readonly IHostingEnvironment currentEnv;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            currentEnv = environment;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(config =>
            {
                config.RootPath = "clientapp/public";
            });
            
            services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts =>
            {
                opts.Cookie.HttpOnly = true;
                opts.Cookie.Name = "AiToolsCookie";
            });

            var assemblyTypes = Assembly.GetAssembly(typeof(DataService)).GetTypes();
            var iDataType = typeof(IDataService);
            var dataType = typeof(DataService);
            //Services DI
            foreach (var servType in assemblyTypes.Where(x => x != dataType && x.IsSubclassOf(dataType)))
            {
                services.AddTransient(assemblyTypes.First(x => x.IsInterface && x.IsAssignableFrom(servType) && x != iDataType), servType);
            }
            assemblyTypes = Assembly.GetAssembly(typeof(Repository<>)).GetTypes();
            var repoType = typeof(Repository<>);
            foreach (var rt in assemblyTypes.Where(x => x != repoType && IsSubclassOfRawGeneric(repoType, x)))
            {
                services.AddTransient(rt);
            }

            services.AddScoped<UserManager>();
            services.AddScoped<SignInManager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddSingleton<FileProvider>();

            if (currentEnv.IsDevelopment())
            {
                services.AddCors();
            }
            //Настраиваем маппинг
            var mapConfig = new MapperConfiguration(mc =>
            {
                //Скан сборки для получения профилей
                mc.AddProfiles(Assembly.GetAssembly(dataType));
            });
            services.AddSingleton(mapConfig.CreateMapper());
        }

        bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder.WithOrigins("http://localhost:4000", "https://localhost:4000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.Map("/clientstate", ClientState);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "clientapp";
            });
        }
        #region Handlers
        static void ClientState(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var services = context.RequestServices;
                var userManager = services.GetRequiredService<UserManager>();

                var authenticated = context.User.Identity.IsAuthenticated;
                User user = authenticated
                    ? await userManager.FindByNameAsync(context.User.Identity.Name)
                    : null;
                IList<string> userRoles = new List<string>();
                if (authenticated)
                    userRoles = await userManager.GetRolesAsync(user);
                var state = new ClientState
                {
                    UserAuthenticated = authenticated,
                    UserName = user == null ? null : $"{user.FirstName} {user.SirName}",
                    UserRoles = userRoles
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(state));
            });
        }
        #endregion
    }
}

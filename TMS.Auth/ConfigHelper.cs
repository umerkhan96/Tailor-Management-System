using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMS.Auth.Contexts;
using TMS.Auth.Dtos;
using TMS.Auth.Repositories.Actions;
using TMS.Auth.Repositories.Interfaces;
using TMS.Auth.Services.Actions;
using TMS.Auth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using TMS.Auth.Seeders;

namespace TMS.Auth
{
    public static class ConfigHelper
    {
        public static IServiceCollection InjectAuthServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<TmsIdentityContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("TmsDb")));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<TmsIdentityContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.IsEssential = true;
                });

            services.AddScoped<SeederManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepo, UserRepo>();

            return services;
        }
    }
}

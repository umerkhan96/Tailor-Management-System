using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TMS.Data.Entities;

namespace TMS.Data
{
    public static class ConfigureData
    {
        public static IServiceCollection InjectData(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<TmsDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("TmsDb")));
            return services;
        }
    }
}

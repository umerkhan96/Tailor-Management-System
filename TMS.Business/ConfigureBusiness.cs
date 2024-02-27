using Microsoft.Extensions.DependencyInjection;
using TMS.Business.Actions;
using TMS.Business.Services;

namespace TMS.Business
{
    public static class ConfigureBusiness
    {
        public static IServiceCollection InjectBusiness(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerActions>();
            services.AddScoped<IOrdersService, OrdersActions>();
            return services;
        }
    }
}

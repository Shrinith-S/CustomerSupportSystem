using CustomerSupport.Application.Interfaces.Services;
using CustomerSupport.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupport.Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITicketCategoryService, TicketCategoryService>();
            services.AddScoped<IMasterDataService, MasterDataService>();

            return services;
        }
    }
}

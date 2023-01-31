using Microsoft.Extensions.DependencyInjection;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Services;

namespace Moryx.Runtime.Endpoints
{
    public static class EndpointsServiceCollectionExtension
    {
        public static IServiceCollection AddEndpoints(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseConfigUpdateService, DatabaseConfigUpdateService>();
            return services;
        }
    }
}

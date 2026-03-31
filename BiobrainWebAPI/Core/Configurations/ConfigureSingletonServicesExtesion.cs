using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureSingletonServicesExtension
    {
        public static IServiceCollection ConfigureSingletonServiceLayer(this IServiceCollection service)
        {
            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return service;
        }
    }
}
using BiobrainWebAPI.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureAppSettingsExtension
    {
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection service, IConfiguration configuration)
        {
            var appSettings = configuration.Get<AppSettings>();
            service.AddSingleton(appSettings);
            return service;
        }
    }
}
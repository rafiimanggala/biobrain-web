using System;
using Biobrain.Application.Interfaces.DataAccess;
using BiobrainWebAPI.Values;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureDataBaseExtension
    {
        private const string MigrationAssembly = "DataAccessLayer";

        public static IServiceCollection ConfigureDatabase(this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddDbContext<BiobrainWebContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(ConfigurationSections.ConnectionStringName),
                    b => b.MigrationsAssembly(MigrationAssembly));
                
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict<Guid>();
            });

            service.AddTransient<IDb, BiobrainWebContext>();

            return service;
        }
    }
}
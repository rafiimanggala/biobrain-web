using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Base
{
    public static class ApplyConfigurationExtension
    {
        public static void ApplyConfigurations(this ModelBuilder modelBuilder)
        {
            var dbAssembly = Assembly.GetAssembly(typeof(ApplyConfigurationExtension));
            modelBuilder.ApplyConfigurationsFromAssembly(dbAssembly);
        }
    }
}

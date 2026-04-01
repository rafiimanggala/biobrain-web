using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BioBrain.Interfaces;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<ICountriesService, CountriesService>();
namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    public class CountriesService : ICountriesService
    {
        public List<string> GetCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
                .Select(c => new RegionInfo(c.LCID))
                .Select(x => x.EnglishName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}

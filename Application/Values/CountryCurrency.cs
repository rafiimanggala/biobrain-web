using System;
using System.Collections.Generic;

namespace Biobrain.Application.Values
{
	public static class CountryCurrency
	{
		public static KeyValuePair<string, string> Get(string country)
		{
			switch (country)
			{
				case "United Kingdom":
					return new KeyValuePair<string, string>("GBP", "£");
				case "Australia":
					return new KeyValuePair<string, string>("AUD", "$");
				default:
					if(EuCountries.Contains(country))
						return new KeyValuePair<string, string>("EUR", "€");
					return new KeyValuePair<string, string>("USD", "$");
			}
		}

		public static string GetSymbolByCode(string code)
        {
            return code switch
            {
                "GBP" => "£",
                "AUD" => "$",
                "EUR" => "€",
                "USD" => "$",
                _ => throw new ArgumentOutOfRangeException(nameof(code))
            };
        }

		private static readonly List<string> EuCountries = new()
		{
			"Austria",
			"Belgium",
			"Bulgaria",
			"Croatia",
			"Cyprus",
			"Czechia",
			"Denmark",
			"Estonia",
			"Finland",
			"France",
			"Germany",
			"Greece",
			"Hungary",
			"Ireland",
			"Italy",
			"Latvia",
			"Lithuania",
			"Luxembourg",
			"Malta",
			"Netherlands",
			"Poland",
			"Portugal",
			"Romania",
			"Slovakia",
			"Slovenia",
			"Spain",
			"Sweden"
		};
	}
}
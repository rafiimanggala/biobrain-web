using System;
using System.Text.RegularExpressions;

namespace Biobrain.Application.Extensions
{
    public static class StringExtension
    {
        public static string RemoveTags(this string value)
        {
            var htmlTagsRegEx = new Regex("<[^>]*>");
            var spacesRegEx = new Regex(@"\s+");
            var raw = htmlTagsRegEx.Replace(value, string.Empty);
            raw = raw.Replace("&nbsp;", " ");
            raw = spacesRegEx.Replace(raw, " ");
            return raw;
        }

        public static string SubstringFromStart(this string value, int symbolsToTake) => value.Substring(0, Math.Min(value.Length, symbolsToTake));
    }
}
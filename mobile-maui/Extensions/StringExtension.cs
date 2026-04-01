using System.Text.RegularExpressions;

namespace BioBrain.Extensions
{
    public static class StringExtension
    {
        private static readonly Regex htmlTagsRegEx = new Regex("<[^>]*>");

        public static string RemoveExtraSpaces(this string str)
        {
            str = str.Trim();
            str = Regex.Replace(str, @"\s+", " ");
            return str;
        }

        public static string RemoveTags(this string str)
        {
            return htmlTagsRegEx.Replace(str, string.Empty);
        }
    }
}

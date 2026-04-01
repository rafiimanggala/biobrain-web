using System;
using System.Linq;
using Common.Enums;

namespace BioBrain.Helpers
{
    public static class StringsHelper
    {
        public static VersionCompareResult CompareVersions(string first, string second)
        {
            if (first == second) return VersionCompareResult.Equals;
            var firstNumber = int.Parse(first);
            var secondNumber = int.Parse(second);
            if (firstNumber > secondNumber)
                return VersionCompareResult.FirstMoreThanSecond;
            if (secondNumber > firstNumber)
                return VersionCompareResult.SecondMoreThanFirst;
            return VersionCompareResult.Equals;
        }

        public static bool IsVersionSupported(string version, string minVersion)
        {
            try
            {
                var parsedVersion = version.Split('.');
                var parsedMinVersion = minVersion.Split('.');
                for (var i = 0; i < parsedVersion.Length; i++)
                {
                    var versionElement = int.Parse(parsedVersion[i]);
                    var minVersionElement = int.Parse(parsedMinVersion[i]);
                    if (minVersionElement > versionElement) return false;
                    if (minVersionElement < versionElement) return true;
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static string GetContext(string sourceText, int startIndex, int length, int contextSize)
        {
            var contextStart = (startIndex - contextSize) > 0 ? (startIndex - contextSize) : 0;
            var contextLength = length + 2 * contextSize;
            contextLength = (contextStart + contextLength) > sourceText.Length
                ? (sourceText.Length - contextStart)
                : contextLength;

            var context = sourceText.Substring(contextStart, contextLength);
            if (contextStart > 0) context = $"...{context}";
            if (contextLength < (sourceText.Length - contextStart)) context = $"{context}...";

            return context;
        }
    }
}
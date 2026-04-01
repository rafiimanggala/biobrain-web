using System.IO;
using System.Reflection;

namespace BioBrain.Helpers
{
    public class ResourceHelper
    {
        public static Stream GetResourceStream(string resource)
        {
            var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(resource);
        }

        public static string GetVersion()
        {
            var resourceName = "BioBrain.AppResources.Version.data";
            return GetStringFromFile(resourceName);
        }

        public static string GetStructureVersion()
        {
            var resourceName = "BioBrain.AppResources.StructureVersion.data";
            return GetStringFromFile(resourceName);
        }

        private static string GetStringFromFile(string path)
        {
            var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null) return "0";
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

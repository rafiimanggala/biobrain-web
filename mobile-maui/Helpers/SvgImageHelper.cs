using System.Reflection;

namespace BioBrain.Helpers
{
    public static class SvgImageHelper
    {
        /// <summary>
        /// Link to assembly for svg image control
        /// </summary>
        public static Assembly SvgAssembly => typeof(App).GetTypeInfo().Assembly;
    }
}

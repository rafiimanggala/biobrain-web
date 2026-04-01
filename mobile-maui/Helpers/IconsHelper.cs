

using BioBrain.AppResources;
using Microsoft.Maui.Controls;

namespace BioBrain.Helpers
{
    public static class IconsHelper
    {
        private static IFilesPath FilePath => DependencyService.Get<IFilesPath>();
        public static ImageSource MainIconPath => new FileImageSource {File = FilePath.IconsPath + "mainicon.png"};
        public static string BackIconPath => FilePath.IconsPath + "backicon.png";
        public static string FooterIconPath => FilePath.IconsPath + "footericon.png";
        public static string ForwardIconPath => FilePath.IconsPath + "forwardicon.png";
        public static string GlossaryIconPath => FilePath.IconsPath + "glossaryicon.png";
        public static string HomeIconPath => FilePath.IconsPath + "homeicon.png";
        public static string PopupIconPath => FilePath.IconsPath + "popupicon.png";
    }
}
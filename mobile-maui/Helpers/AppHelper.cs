using Microsoft.Maui.Controls;

namespace BioBrain.Helpers
{
    public static class AppHelper
    {
        public static string AppVersion = DeviceInfo.Platform == DevicePlatform.iOS
            ? AppInfo.Current.VersionString.Substring(0, AppInfo.Current.VersionString.Length / 2)
            : AppInfo.Current.VersionString;
    }
}

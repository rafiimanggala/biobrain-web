// TODO: Version.Plugin (Xam.Plugin.VersionTracking) does not exist for MAUI.
// Use Microsoft.Maui.ApplicationModel.VersionTracking instead.
// This stub allows compilation; calls should be replaced with MAUI equivalents.

namespace Version.Plugin
{
    public static class CrossVersion
    {
        public static IVersion Current => new MauiVersionAdapter();
    }

    public interface IVersion
    {
        string Version { get; }
        string Build { get; }
    }

    internal class MauiVersionAdapter : IVersion
    {
        public string Version => Microsoft.Maui.ApplicationModel.AppInfo.VersionString;
        public string Build => Microsoft.Maui.ApplicationModel.AppInfo.BuildString;
    }
}

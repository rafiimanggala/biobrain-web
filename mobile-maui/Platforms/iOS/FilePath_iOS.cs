using System;
using System.IO;
using BioBrain.AppResources;

namespace BioBrain.Platforms.iOS
{
    public class FilePath_iOS : IFilesPath
    {
#if Biology
#if US
        public const string AppID = "1423499530";
#elif EU
        public const string AppID = "1453129963";
#else
        public const string AppID = "1226261595";
#endif
#elif Chemistry
#if US
        public const string AppID = "1453130122";
#elif EU
        public const string AppID = "1473780385";
#else
        public const string AppID = "1410639674";
#endif
#elif Physics
#if US
        public const string AppID = "1510694550";
#elif EU
        public const string AppID = "1510695664";
#else
        public const string AppID = "1492905132";
#endif
#endif

        public static string AppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", "Caches");
        public static string AvatarFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");
        public static string HtmlPath => Path.Combine(AppPath, "html");
        public static string DbFilePath => Path.Combine(AppPath, "BioBrain_Content.db3");
        public static string AccountFilePath => Path.Combine(AppPath, "acc.json");
        public static string DemoDbFilePath => Path.Combine(AppPath, "Demo_BioBrain_Content.db3");
        public static string VersionPath => Path.Combine(AppPath, "Version.data");
        public static string StructVersionPath => Path.Combine(AppPath, "StructureVersion.data");
        public static string IconsPath => Path.Combine(PagesPath, "Images");
        public static string ImagesPath => Path.Combine(PagesPath, "Images");
        public static string PagesPath => Path.Combine(AppPath, "html");
        public static string LogPath => Path.Combine(AppPath, "log_file.txt");
        public static string OldLogPath => Path.Combine(AppPath, "log.txt");
        public string DatabasePath => DbFilePath;
        public string DemoDatabasePath => DemoDbFilePath;

        public static string MarketUrl => $"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={AppID}&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software";
        public static string DemoMarketUrl => $"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={AppID}&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software";

        string IFilesPath.MarketUrl => MarketUrl;
        string IFilesPath.DemoMarketUrl => DemoMarketUrl;
        string IFilesPath.ImagesPath => ImagesPath;
        string IFilesPath.PagesPath => PagesPath;
        string IFilesPath.IconsPath => IconsPath;
        string IFilesPath.AppPath => AppPath;
        string IFilesPath.AvatarPath => AvatarFolderPath;
        string IFilesPath.HtmlFolderPath => HtmlPath;
        public string DataVersionPath => VersionPath;
        public string StructureVersionPath => StructVersionPath;
        public string InternetCache => AppPath;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Interfaces;
using Common.Enums;
using Common.Interfaces;
using DAL;
using DAL.Models.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using SQLite;

namespace BioBrain.Stubs
{
    /// <summary>
    /// No-op IErrorLog for development. Logs to Console.
    /// </summary>
    public class ConsoleErrorLog : IErrorLog
    {
        public void Log(string text) => Console.WriteLine($"[ErrorLog] {text}");
        public string GetLog() => string.Empty;
        public bool OldLogFileExist() => false;
        public void Init() { }
    }

    /// <summary>
    /// No-op IAnalyticTracker for development.
    /// </summary>
    public class NoOpAnalyticTracker : IAnalyticTracker
    {
        public void SetView(string screenName, string className) { }
        public void SendEvent(string eventId) { }
        public void SendEvent(string eventId, string paramName, string value) { }
        public void SendEvent(string eventId, IDictionary<string, string> parameters) { }
    }

    /// <summary>
    /// Stub IFilesPath returning platform-appropriate paths.
    /// </summary>
    public class StubFilesPath : IFilesPath
    {
        private readonly string _base = FileSystem.AppDataDirectory;

        public string DatabasePath => Path.Combine(_base, "biobrain.db3");
        public string DemoDatabasePath => Path.Combine(_base, "biobrain_demo.db3");
        public string AppPath => _base;
        public string IconsPath => Path.Combine(_base, "Icons");
        public string ImagesPath => Path.Combine(_base, "Images");
        public string PagesPath => Path.Combine(_base, "Pages");
        public string MarketUrl => "https://apps.apple.com";
        public string DemoMarketUrl => "https://apps.apple.com";
        public string AvatarPath => Path.Combine(_base, "avatar.png");
        public string HtmlFolderPath => Path.Combine(_base, "Html");
        public string DataVersionPath => Path.Combine(_base, "version.txt");
        public string StructureVersionPath => Path.Combine(_base, "struct_version.txt");
        public string InternetCache => Path.Combine(_base, "Cache");
    }

    /// <summary>
    /// Stub IProjectDataWorker — no real file operations.
    /// </summary>
    public class StubProjectDataWorker : IProjectDataWorker
    {
        private readonly string _base = FileSystem.AppDataDirectory;
        public bool IsUpdateNeeded(int newVersion) => false;
        public bool IsStructUpdateNeeded(int newStructVersion) => false;
        public bool IsStructChanged(int newStructVersion) => false;
        public bool FullDataBaseExist() => File.Exists(Path.Combine(_base, "biobrain.db3")) && new FileInfo(Path.Combine(_base, "biobrain.db3")).Length > 0;
        public bool DemoDataBaseExist()
        {
            var path = Path.Combine(_base, "biobrain_demo.db3");
            if (!File.Exists(path) || new FileInfo(path).Length == 0) return false;
            try
            {
                using var conn = new SQLite.SQLiteConnection(path);
                var count = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Areas");
                return count > 0;
            }
            catch { return false; }
        }
        public void DeleteExistingDemo() { }
        public void DeleteFile(string filename) { }
        public void StoreVersionFile(Stream fileStream) { }
        public void StoreStructureVersionFile(Stream fileStream) { }
        public bool CheckFileSize(string path, int size) => false;
        public bool ReplaceFileText(string path, string text) => false;
        public void MoveFile(string fromPath, string toPath) { }
        public void CreateDiectory(string path) { }
        public string GetDataStoredVersion() => "0";
    }

    /// <summary>
    /// Stub ISQLite returning in-memory connections.
    /// </summary>
    public class StubSQLite : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "biobrain.db3");
            return new SQLiteConnection(path);
        }

        public SQLiteConnection GetDemoConnection()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "biobrain_demo.db3");
            return new SQLiteConnection(path);
        }
    }

    /// <summary>
    /// Stub IWorkingWithFiles — creates temp HTML files.
    /// </summary>
    public class StubWorkingWithFiles : IWorkingWithFiles
    {
        public string CreateHtmlFile(string fileName, string html)
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, fileName);
            File.WriteAllText(path, html);
            return path;
        }

        public string MoveFileToAppDirectory(string filePath) => filePath;
    }

    /// <summary>
    /// Stub IAccountDataProvider using Preferences.
    /// </summary>
    public class StubAccountDataProvider : IAccountDataProvider
    {
        public string GetAccountData() => Preferences.Get("account_data", string.Empty);
        public void SetAccountData(string json) => Preferences.Set("account_data", json);
    }

    /// <summary>
    /// Stub IAccountDataStoreManager using Preferences.
    /// </summary>
    public class StubAccountDataStoreManager : IAccountDataStoreManager
    {
        public void SaveUserRateResult(RateResult result) => Preferences.Set("rate_result", (int)result);
        public void SaveEnterNumber(int enterNumber) => Preferences.Set("enter_number", enterNumber);
        public RateResult GetUserRateResult() => (RateResult)Preferences.Get("rate_result", 0);
        public int GetEnterNumber() => Preferences.Get("enter_number", 0);
    }

    /// <summary>
    /// Stub IActionSheet using DisplayActionSheet.
    /// </summary>
    public class StubActionSheet : IActionSheet
    {
        public async Task<string> UseActionSheet(Page p, string title, string cancel, params string[] buttons)
        {
            return await p.DisplayActionSheet(title, cancel, null, buttons) ?? cancel;
        }
    }

    /// <summary>
    /// Stub IApplicationParamsProvider.
    /// </summary>
    public class StubApplicationParamsProvider : IApplicationParamsProvider
    {
        public bool IsPickImage { get; set; }
    }

    /// <summary>
    /// No-op IClassKit for non-iOS or development.
    /// </summary>
    public class NoOpClassKit : IClassKit
    {
        public void SetupClassKitContexts(List<IAreaModel> areasToAdd, List<ITopicModel> topicsToAdd, List<IMaterialModel> materialsToAdd, List<ILevelTypeModel> levelTypes) { }
        public void StartActivity(List<string> identifierPath, bool asNew = false) { }
        public void UpdateProgress(string[] identifierPath, double progress) { }
        public void AddScore(string[] identifierPath, double score, double maxScore, string title, bool primary = false) { }
        public void StopActivity(string[] identifierPath) { }
    }

    /// <summary>
    /// Stub ICountriesService returning empty list.
    /// </summary>
    public class StubCountriesService : ICountriesService
    {
        public List<string> GetCountries() => new List<string>
        {
            "Australia", "United States", "United Kingdom", "Canada", "New Zealand"
        };
    }

    /// <summary>
    /// No-op IDecryption — returns the input stream unchanged.
    /// </summary>
    public class NoOpDecryption : IDecryption
    {
        public Stream Def(Stream inFile, string password) => inFile;
    }

    /// <summary>
    /// No-op IEmailSender.
    /// </summary>
    public class NoOpEmailSender : IEmailSender
    {
        public void Compose(EmailMessage message, string textMessage)
        {
            Console.WriteLine($"[EmailSender] Would send: {textMessage}");
        }
    }

    /// <summary>
    /// No-op IOrientationService.
    /// </summary>
    public class NoOpOrientationService : IOrientationService
    {
        public void Landscape() { }
        public void All() { }
        public void Portrait() { }
    }

    /// <summary>
    /// Stub IRateApp — always returns NextTime.
    /// </summary>
    public class StubRateApp : IRateApp
    {
        public Task<RateResult> RateApp() => Task.FromResult(RateResult.NextTime);
    }

    /// <summary>
    /// No-op IZipResource.
    /// </summary>
    public class NoOpZipResource : IZipResource
    {
        public void Unzip(Stream zipStream, string dstPath)
        {
            Console.WriteLine($"[ZipResource] Would unzip to: {dstPath}");
        }
    }
}

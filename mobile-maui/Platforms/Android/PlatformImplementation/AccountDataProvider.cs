using System.IO;
using Common.Interfaces;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IAccountDataProvider, AccountDataProvider>();
namespace BioBrain.Platforms.Android.PlatformImplementation
{
    public class AccountDataProvider : IAccountDataProvider
    {
        public string GetAccountData()
        {
            // TODO: Replace FilePathDroid with MAUI FileSystem.AppDataDirectory
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "account.json");
            if (!File.Exists(filePath)) return null;
            var data = File.ReadAllText(filePath);
            return data;
        }

        public void SetAccountData(string json)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "account.json");
            File.WriteAllText(filePath, json);
        }
    }
}

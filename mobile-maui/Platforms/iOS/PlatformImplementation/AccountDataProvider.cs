using System.IO;
using BioBrain.Platforms.iOS;
using Common.Interfaces;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IAccountDataProvider, AccountDataProvider>();
namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    public class AccountDataProvider : IAccountDataProvider
    {
        public string GetAccountData()
        {
            if (!File.Exists(FilePath_iOS.AccountFilePath)) return null;
            var data = File.ReadAllText(FilePath_iOS.AccountFilePath);
            return data;
        }

        public void SetAccountData(string json)
        {
            File.WriteAllText(FilePath_iOS.AccountFilePath, json);
        }
    }
}

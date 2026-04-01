using BioBrain.Interfaces;
using Foundation;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IBaseUrl, BaseUrl_iOS>();
namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    public class BaseUrl_iOS : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}

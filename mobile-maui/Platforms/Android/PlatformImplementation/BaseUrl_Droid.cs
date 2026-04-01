using BioBrain.Interfaces;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IBaseUrl, BaseUrl_Android>();
namespace BioBrain.Platforms.Android.PlatformImplementation
{
    public class BaseUrl_Android : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}

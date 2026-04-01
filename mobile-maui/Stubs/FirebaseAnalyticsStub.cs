// TODO: Firebase Analytics stubs for MAUI migration.
// Replace with actual Firebase Analytics MAUI package when available.
// Consider: Plugin.Firebase.Analytics or Microsoft.Maui Firebase community bindings.

#if IOS
using Foundation;

namespace Firebase.Analytics
{
    public static class Analytics
    {
        public static void LogEvent(string name, NSDictionary<NSString, NSObject>? parameters)
        {
            // TODO: Implement with actual Firebase Analytics iOS binding
            System.Diagnostics.Debug.WriteLine($"[Firebase Analytics Stub] LogEvent: {name}");
        }

        public static void SetScreenNameAndClass(string screenName, string className)
        {
            // TODO: Implement with actual Firebase Analytics iOS binding
            System.Diagnostics.Debug.WriteLine($"[Firebase Analytics Stub] SetScreen: {screenName}/{className}");
        }
    }
}
#elif ANDROID
namespace Firebase.Analytics
{
    public class FirebaseAnalytics
    {
        private static FirebaseAnalytics? _instance;

        public static FirebaseAnalytics GetInstance(Android.Content.Context context)
        {
            _instance ??= new FirebaseAnalytics();
            return _instance;
        }

        public void LogEvent(string name, Android.OS.Bundle? parameters)
        {
            // TODO: Implement with actual Firebase Analytics Android binding
            System.Diagnostics.Debug.WriteLine($"[Firebase Analytics Stub] LogEvent: {name}");
        }

        public void SetCurrentScreen(Android.App.Activity activity, string screenName, string className)
        {
            // TODO: Implement with actual Firebase Analytics Android binding
            System.Diagnostics.Debug.WriteLine($"[Firebase Analytics Stub] SetScreen: {screenName}/{className}");
        }
    }
}
#endif

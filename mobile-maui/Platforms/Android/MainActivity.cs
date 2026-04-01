using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase.Analytics;

namespace BioBrain;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    FirebaseAnalytics firebaseAnalytics;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Initialize Firebase Analytics
        firebaseAnalytics = FirebaseAnalytics.GetInstance(this);

        // Initialize AppCenter
        // TODO: Add Microsoft.AppCenter.Maui NuGet package
        // Microsoft.AppCenter.AppCenter.Start("b5c45a2c-9047-4626-9b2b-e40fefccb824",
        //     typeof(Microsoft.AppCenter.Analytics.Analytics),
        //     typeof(Microsoft.AppCenter.Crashes.Crashes));
    }
}

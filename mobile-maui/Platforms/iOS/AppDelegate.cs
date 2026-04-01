using Foundation;
using UIKit;

namespace BioBrain;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // TODO: Initialize Firebase when Firebase iOS SDK NuGet is added
        // Firebase.Core.App.Configure();

        // Initialize AppCenter
        // TODO: Add Microsoft.AppCenter.Maui NuGet package
        // Microsoft.AppCenter.AppCenter.Start("694255f5-7565-476e-950e-754303353802",
        //     typeof(Microsoft.AppCenter.Analytics.Analytics),
        //     typeof(Microsoft.AppCenter.Crashes.Crashes));

        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("application:supportedInterfaceOrientationsForWindow:")]
    public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
    {
        // Allow landscape for ElementsTableView
        // TODO: Update navigation check for MAUI Shell/NavigationPage pattern
        var currentPage = Microsoft.Maui.Controls.Application.Current?.MainPage?.Navigation?.NavigationStack;
        if (currentPage != null && currentPage.Count > 0)
        {
            var topPage = currentPage[currentPage.Count - 1];
            if (topPage?.GetType().Name == "ElementsTableView")
                return UIInterfaceOrientationMask.Landscape;
        }

        return UIInterfaceOrientationMask.All;
    }

    public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity,
        UIApplicationRestorationHandler completionHandler)
    {
        // Handle ClassKit deep links
        if (userActivity.IsClassKitDeepLink)
        {
            // TODO: Wire up ClassKit deep link navigation for MAUI
            // App.InitialNavigationPath = userActivity.ContextIdentifierPath.Skip(1).ToList();
            return true;
        }

        return false;
    }
}

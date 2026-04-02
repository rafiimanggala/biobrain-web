using System.Linq;
using Foundation;
using UIKit;

namespace BioBrain;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        try
        {
            Firebase.Core.App.Configure();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Firebase] Init failed: {ex.Message}");
        }

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
            App.InitialNavigationPath = userActivity.ContextIdentifierPath?.Skip(1).ToList();
            return true;
        }

        return false;
    }
}

using BioBrain.Services.Implementations;
using BioBrain.Services.Interfaces;
using BioBrain.Views;
using Common.Interfaces;
using Unity;

namespace BioBrain;

public partial class App : Application
{
    /// <summary>
    /// Unity container compatibility shim backed by MAUI's IServiceProvider.
    /// All legacy App.Container.Resolve calls route through this.
    /// </summary>
    public static IUnityContainer Container { get; private set; } = null!;

    /// <summary>
    /// Static reference to FirebaseService for legacy compatibility.
    /// In the Xamarin version this was resolved via Unity container.
    /// </summary>
    public static FirebaseService? Authorize { get; private set; }

    /// <summary>
    /// Whether an in-app purchase has been made this session.
    /// </summary>
    public static bool IsPurchaseMade { get; set; } = false;

    /// <summary>
    /// Whether the rate dialog has been shown this session.
    /// </summary>
    public static bool IsRateShown { get; set; } = false;

    /// <summary>
    /// Application display name.
    /// </summary>
    public static string AppName => "BioBrain";

    /// <summary>
    /// Navigation path to restore on resume (if any).
    /// </summary>
    public static List<string>? InitialNavigationPath { get; set; }

    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;

        // Initialize screen density for SkiaSharp controls
        Common.Settings.Init((float)DeviceDisplay.MainDisplayInfo.Density);

        // Initialize Unity compatibility container backed by MAUI DI
        Container = new ServiceProviderContainer(serviceProvider, MauiProgram.ServiceDescriptors);

        // Resolve FirebaseService singleton (mirrors old Unity Container.Resolve)
        Authorize = _serviceProvider.GetService<FirebaseService>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Extract bundled demo database before any page loads
        try
        {
            AppResources.UpdateManager.InitialUpdateFiles();
            Common.Settings.ContentType = AppResources.UpdateManager.GetLocalContentType();
            Console.WriteLine($"[App] Demo DB initialized, ContentType: {Common.Settings.ContentType}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[App] InitialUpdateFiles error: {ex.Message}");
        }

        Page startPage;
        try
        {
#if DEBUG
            // In DEBUG, skip login and go straight to AreasView for testing
            startPage = _serviceProvider.GetRequiredService<AreasView>();
#else
            // Check if user is already authenticated
            if (Authorize?.IsAuthorized == true)
            {
                startPage = _serviceProvider.GetRequiredService<AreasView>();
            }
            else
            {
                startPage = _serviceProvider.GetRequiredService<AuthorizationView>();
            }
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[App] Failed to resolve start page: {ex.Message}");
            // Fallback to login page
            startPage = new ContentPage
            {
                Title = "BioBrain",
                Content = new Label
                {
                    Text = $"Error loading: {ex.Message}",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
        }

        return new Window(new NavigationPage(startPage));
    }

    public static async Task Sleep(int ms)
    {
        await Task.Delay(ms);
    }

    /// <summary>
    /// Starts the background update checker service for periodic update and subscription checks.
    /// </summary>
    public void StartUpdateChecker()
    {
        var service = _serviceProvider.GetService<IBackgroundWorkerService>();
        service?.StartPeriodically();
    }

    protected override void OnResume()
    {
        base.OnResume();
        // TODO: Restore InitialNavigationPath handling once NavigationExtension is wired up
    }
}

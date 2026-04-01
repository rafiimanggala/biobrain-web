using System;
using System.IO;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls;
using CustomControls.LayoutControls;
using Unity;
using Unity.Resolution;
using Version.Plugin;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AreasView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        private bool isAlerted = false;

        private IAreasOfStudyViewModel ViewModel {
            get => (IAreasOfStudyViewModel) BindingContext;
            set => BindingContext = value;
        }

        public AreasView(IAreasOfStudyViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            logger.Log("AreasView");
            IsAreasView = true;
            analyticTracker.SetView("Areas Page", nameof(AreasView));
            BackgroundColor = CustomColors.LightMain;
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.GetFullData += ViewModelOnGetFullData;
            ViewModel.NoEmailError += ViewModelOnNoEmailError;
            ViewModel.UpdateNeed += OnUpdateNeed;
            Purchase += OnPurchaseMade;

            AreasList.ItemSelected += (sender, e) =>
            {
                ((ListView)sender).SelectedItem = null;
            };

            ComingSoonAreasList.ItemSelected += (sender, e) =>
            {
                ((ListView)sender).SelectedItem = null;
            };

            //if (Device.RuntimePlatform != Device.Android) return;
            //var appParams = DependencyService.Get<IApplicationParamsProvider>();
            //if (appParams.IsPickImage)
            //{
            //    Navigation.MoveToAccountWithHistoryRestore();
            //}
            //appParams.IsPickImage = false;
#if  Chemistry
            LabelText.WidthRequest = 154;
#endif
#if  Physics
            LabelText.WidthRequest = 133;
#endif
        }

        private async void ViewModelOnGetFullData(object sender, EventArgs e)
        {
#if DEBUG
            logger.Log("DEBUG: Skipping update navigation (no backend in debug)");
#else
	        await Navigation.MoveToUpdates(true);
#endif
        }

        private async void OnPurchaseMade(object sender, EventArgs e)
        {
	        App.IsPurchaseMade = true;
	        await ViewModel.SyncData();
        }

        protected override async void OnAppearing()
        {
            BaseGrid.Padding = Device.RuntimePlatform == Device.iOS ? new Thickness(0, -Padding.Top, 0, -Padding.Bottom) : new Thickness(0, -25, 0, 0);

            SetControlProperties();
            base.OnAppearing();
            logger.Log($"AreasView - AppVersion: {CrossVersion.Current.Version}");

            await Task.Run(() =>
            {
	            UpdateManager.InitialUpdateFiles();
	            Settings.ContentType = UpdateManager.GetLocalContentType();
            });

            var syncSuccess = false;
            try { syncSuccess = await ViewModel.SyncData(); }
            catch (Exception ex) { logger.Log($"SyncData error: {ex.Message}"); }
            finally
            {
                // Force IsBusy=false so content area becomes visible
                if (ViewModel.IsBusy) ViewModel.ForceEndProcess();
            }

            logger.Log($"SyncData done — Sync:{syncSuccess} Busy:{ViewModel.IsBusy} Show:{ViewModel.IsShowData} Lock:{ViewModel.IsLocked}");

            // Always try to load areas even if sync fails (offline/demo mode)
            BackgroundColor = CustomColors.DarkMain;
            BaseGrid.Padding = new Thickness(0, 0, 0, 0);
            try
            {
                ViewModel.GetAreas();
                logger.Log($"GetAreas loaded {ViewModel.Areas.Count} areas");
            }
            catch (Exception ex)
            {
                logger.Log($"GetAreas failed: {ex.Message}");
            }
            await Navigation.HandleInitialNavigation();

            var paths = DependencyService.Get<IFilesPath>();
            logger.Log($"Is {Path.Combine(paths.ImagesPath, "aoc121.svg")} exist: {File.Exists(Path.Combine(paths.ImagesPath, "aoc121.svg"))}");

#if !DEBUG
            if (Settings.IsDemo && !isAlerted)
            {
	            await Alert();
	            isAlerted = true;
            }
#endif

            //SearchContentBar.Invalidate();
        }

        private async void OnUpdateNeed(object sender, string message)
        {
	        if (await DisplayAlert(StringResource.UpgradeString, message,
		        StringResource.NavigateToString +
		        $" {(Device.RuntimePlatform == Device.iOS ? StringResource.AppStore : StringResource.GooglePlay)}",
		        StringResource.CancelString))
	        {

		        var paths = DependencyService.Get<IFilesPath>();
		        await Launcher.OpenAsync(new Uri(paths.MarketUrl));
	        }
	        Application.Current.Quit();
        }

        private async void ViewModelOnNoEmailError(object sender, EventArgs eventArgs)
        {
            await Navigation.MoveToAuthorization();
            //if (await DisplayAlert(StringResource.UpdateString, StringResource.SelectEmailString,
            //    StringResource.AddEmailString, StringResource.CancelString))
            //    await ViewModel.GetEmailAndRetry();
        }

        private async void ViewModelOnAppVersionToLow(object sender, AppVersionEventArgs e)
        {
            if (!await
                DisplayAlert(StringResource.AppVersion, StringResource.AppVersionError, StringResource.UpdateString,
                    StringResource.CancelString)) return;
            var paths = DependencyService.Get<IFilesPath>();
            await Launcher.OpenAsync(new Uri(paths.MarketUrl));
        }

        private void SetControlProperties()
        {
            //LabelBackground.Margin = new Thickness(0,Height/2+229,0,0);
            //LabelText.Margin = new Thickness(0,Height/2+251,0,0);
        }

        private void Button_OnClicked(object sender, ItemTappedEventArgs e)
        {
            Popup.ClosePopup();
            var dataContext = (IAreaViewModel)e.Item;
            if(dataContext == null) return;
            GoToTopic(dataContext.Id);
        }

        private async void GoToTopic(int id)
        {
            AreasList.IsEnabled = false;
            if (!ViewModel.CouldNavigateToArea(id))
            {
	            await Alert();
	            AreasList.IsEnabled = true;
                return;
            }

            await Navigation.PushAsync(App.Container.Resolve<TopicsView>(string.Empty, new ParameterOverride("areaOfStudyId", id)), false);
            //Hack to fix out of memory
            GC.Collect();
            AreasList.IsEnabled = true;
        }

        private void FakeTap(object sender, EventArgs e)
        {
            //Hack for ios
        }

        private void SearchBar_OnOnSearch(object sender, SearchEventArgs e)
        {
            if(string.IsNullOrEmpty(e.SearchText) || e.SearchText.Length < 2) return;
            analyticTracker?.SendEvent(AnalyticEvents.AreasViewSearched, AnalyticEventParams.SearchText, e.SearchText);
            Navigation.PushAsync(App.Container.Resolve<SearchView>(new ParameterOverride("text", e.SearchText)), false);
            SearchContentBar.Text = string.Empty;
        }

        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            Popup.ClosePopup();
        }

        private void TopBaner_OnShare(object sender, EventArgs e)
        {
            if (Popup.IsVisible)
            {
                Popup.ClosePopup();
            }
            else
            {
                Popup.InitPopup();
            }
        }
    }
}

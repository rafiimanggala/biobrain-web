using System;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopicsView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private ITopicsViewModel ViewModel
        {
            get => (ITopicsViewModel)BindingContext;
            set => BindingContext = value;
        }

        public TopicsView(ITopicsViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            logger.Log("TopicsView");
            analyticTracker.SetView("Topics Page", nameof(TopicsView));
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.Error += ViewModelOnError;
            ViewModel.AuthError += ViewModelOnAuthError;
            TopicsList.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
        }

        private async void ViewModelOnAuthError(object sender, string s)
        {
            //if (await DisplayAlert(StringResource.EmailString, StringResource.SelectEmailString,
            //              StringResource.AddEmailString, StringResource.CancelString))
            //    await ViewModel.GetEmailAndRetry();
            await Navigation.MoveToAuthorization();
            TopicsList.IsEnabled = true;
        }

        private async void ViewModelOnError(object sender, string s)
        {
            await DisplayAlert(StringResource.ErrorString, s, StringResource.OkString);
            TopicsList.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("TopicsView");
            try
            {
                ViewModel.GetTopics();
            }
            catch (Exception ex)
            {
                logger.Log($"TopicsView OnAppearing error: {ex.Message}");
            }
        }

        private void Button_OnClicked(object sender,  ItemTappedEventArgs e)
        {
            Popup.ClosePopup();
            var datacontext = (ITopicViewModel)e.Item;
            if (datacontext == null) return;
            //GoToTopic(datacontext.TopicID);
            GoToMaterial(datacontext.MaterialID);
        }

        private async void GoToMaterial(int materialID)
        {
            TopicsList.IsEnabled = false;
            if (materialID == -1)
            {
	            if (!CrossConnectivity.Current.IsConnected)
	            {
		            OnError(this, StringResource.NoInternetError);
		            return;
	            }
                await Alert();
                TopicsList.IsEnabled = true;
                return;
            }
            await Navigation.GoToMaterial(materialID);
            TopicsList.IsEnabled = true;
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

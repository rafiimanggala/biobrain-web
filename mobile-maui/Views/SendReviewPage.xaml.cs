using System;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendReviewPage
    {
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private ISendReviewViewModel ViewModel
        {
            get => (ISendReviewViewModel)BindingContext;
            set => BindingContext = value;
        }

        public SendReviewPage(ISendReviewViewModel viewModel) : base(MenuItemsEnum.More)
        {
            analyticTracker.SetView("Send App Review Page", nameof(SendReviewPage));
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void CancelButton_OnClicked(object sender, object e)
        {
            ViewModel.SaveRateResult(RateResult.NextTime);
            await Navigation.PopAsync(false);
        }

        private async void SendButton_OnClicked(object sender, object e)
        {
            await ViewModel.SendReview();
            await Navigation.PopAsync(false);
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
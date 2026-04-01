using System;
using BioBrain.AppResources;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class ReviewView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public ReviewView() : base(MenuItemsEnum.Home)
        {
            logger.Log("ReviewView C");
            analyticTracker.SetView("Rate The App Page", nameof(ReviewView));
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("ReviewView A");
        }

        private async void LeaveAReviewButton_OnClicked(object sender, object e)
        {
            try
            {
                var paths = DependencyService.Get<IFilesPath>();
                await Launcher.OpenAsync(new Uri(paths.MarketUrl));
            }
            catch (Exception)
            {
                logger.Log("ReviewView OpenUrlError");
            }
        }

        private async void MaybeLaterButton_OnClicked(object sender, object e)
        {
            await Navigation.MoveBack();
        }

        private void RateButton_OnClicked(object sender, object e)
        {
            //if(Device.OS == TargetPlatform.Android) return;
            var rater = DependencyService.Get<IRateApp>();
            rater.RateApp();
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

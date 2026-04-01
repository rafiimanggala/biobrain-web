using System;
using BioBrain.AppResources;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class FeedbackView: IBaseView
    {
        private readonly IFeedbackViewModel feedbackViewModel;
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        public FeedbackView(IFeedbackViewModel feedbackViewModel) : base(MenuItemsEnum.More)
        {
            logger.Log("FeedbackView");
            analyticTracker.SetView("Feedback Page", nameof(FeedbackView));
            InitializeComponent();
            this.feedbackViewModel = feedbackViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("FeedbackView");
        }

        private async void LikeButton_OnClicked(object sender, object e)
        {
            await Navigation.PushAsync(new ReviewView(), false);
        }

        private async void SuggestionButton_OnClicked(object sender, object e)
        {
            await Launcher.OpenAsync(new Uri($"mailto:{Links.SupportEmail}?subject={StringResource.SuggestionSubjectString}"));
        }

        private async void ProblemButton_OnClicked(object sender, object e)
        {
            await Launcher.OpenAsync(new Uri($"mailto:{Links.SupportEmail}?subject={StringResource.ProblemSubjectString}"));
        }

        private async void SendLog_Clicked(object sender, object e)
        {
            var message = await feedbackViewModel.SendLog();
            await DisplayAlert(StringResource.LogsString, message, StringResource.OkString);
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

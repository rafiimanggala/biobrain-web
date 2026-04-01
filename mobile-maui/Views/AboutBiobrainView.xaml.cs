using System;
using BioBrain.AppResources;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Unity;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class AboutBiobrainView
    {
        private IAboutBiobrainViewModel ViewModel
        {
            get => (IAboutBiobrainViewModel)BindingContext;
            set => BindingContext = value;
        }

        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public AboutBiobrainView(IAboutBiobrainViewModel aboutBiobrainViewModel) : base(MenuItemsEnum.More)
        {
            logger.Log("AboutBiobrainView");
            analyticTracker.SetView("More Tab", nameof(AboutBiobrainView));
            InitializeComponent();
            ViewModel = aboutBiobrainViewModel;
            ViewModel.Error+= ViewModelOnError;
            ViewModel.AuthError += ViewModelOnAuthError;
            Settings.DataUpdateChanged += SettingsOnDataUpdateChanged;
        }

        private void SettingsOnDataUpdateChanged(object sender, bool e)
        {
	        UpdateBadge.IsVisible = e;
        }

        private async void ViewModelOnAuthError(object sender, string s)
        {
            //if (await DisplayAlert(StringResource.EmailString, StringResource.SelectEmailString,
            //              StringResource.AddEmailString, StringResource.CancelString))
            //    await ViewModel.GetEmailAndRetrySend();
            await Navigation.MoveToAuthorization();
        }

        private async void ViewModelOnError(object sender, string s)
        {
            await DisplayAlert(StringResource.ErrorString, s, StringResource.OkString);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("AboutBiobrainView");
        }

        public async void ActivatePromo()
        {
            if (!ViewModel.IsSendPromocod) return;

            if (await ViewModel.ActivatePromo())
                await Navigation.MoveToAreasAndInitUpdate();
            else
                await
                    DisplayAlert(StringResource.PromoCodeString, StringResource.PromoCodeError,
                        StringResource.OkString);
        }

        private async void VisitHelpButton_OnClicked(object sender, object e)
        {
            if (Popup.IsVisible) return;
            await Navigation.MoveToAbout();
        }

        private async void GiveFeedbackButton_OnClicked(object sender, object e)
        {
            if (Popup.IsVisible) return;
            await Navigation.PushAsync(App.Container.Resolve<FeedbackView>(), false);
        }

        private async void PrivacyPolicyButton_OnClicked(object sender, object e)
        {
            if (Popup.IsVisible) return;
            await Navigation.MoveToTextView(FileNames.PrivacyPolicy, StringResource.PrivacyPolicyString);
        }

        private async void TermsOfServiceButton_OnClicked(object sender, object e)
        {
            if (Popup.IsVisible) return;
            await Navigation.MoveToTextView(FileNames.TermsOfService, StringResource.TermsOfServiceString);
        }

        private async void AboutButton_OnClicked(object sender, object e)
        {
            if (Popup.IsVisible) return;
            await Navigation.PushAsync(App.Container.Resolve<AboutView>(), false);
        }

        private async void YourAccountButton_OnClicked(object sender, object e)
        {
            if(Popup.IsVisible) return;
            await Navigation.MoveToAccount();
        }

        private void PromoCodeButton_OnClicked(object sender, object e)
        {
            if (!ViewModel.CheckIsAuthorized()) return;
            Popup.IsVisible = true;
        }

        private void EnterPromoCode_OnClicked(object sender, object e)
        {
            if (!ViewModel.CheckIsAuthorized()) return;
            ViewModel.SendPromocode();
            Popup.IsVisible = false;
        }

        private async void UpdatesButton_OnClicked(object sender, object e)
        {
	        if (Popup.IsVisible) return;
	        await Navigation.MoveToUpdates();
        }

        private void ClosePopup_OnClicked(object sender, object e)
        {
            Popup.IsVisible = false;
        }

        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            if (Popup.IsVisible) e.Cancel = true;
        }

        private void TopBaner_OnShare(object sender, EventArgs e)
        {
            if (Popup.IsVisible) return;
            if (SharePopup.IsVisible)
            {
                SharePopup.ClosePopup();
            }
            else
            {
                SharePopup.InitPopup();
            }
        }
    }
}

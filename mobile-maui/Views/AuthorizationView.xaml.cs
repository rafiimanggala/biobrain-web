using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class AuthorizationView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private IAuthorizationViewModel ViewModel
        {
            get => (IAuthorizationViewModel)BindingContext;
            set => BindingContext = value;
        }

        public AuthorizationView(IAuthorizationViewModel viewModel) : base(MenuItemsEnum.More)
        {
            logger.Log("RegisterPage");
            analyticTracker.SetView("Register Page", nameof(AccountView));
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.Error += ViewModelOnError;
            ViewModel.Finish += ViewModelOnFinish;
            ViewModel.ResetPasswordEmailSent += ViewModelOnResetPasswordEmailSent;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ViewModelOnResetPasswordEmailSent(object sender, EventArgs e)
        {
            await DisplayAlert(StringResource.ForgotPasswordString, StringResource.ResetEmailSentString,
                StringResource.OkString);
            ViewModel.SetLoginMode();
        }

        private async void ViewModelOnFinish(object sender, EventArgs e)
        {
            await Navigation.MoveToAreasAfterAuth();
        }

        private void ViewModelOnError(object sender, string e)
        {
            DisplayAlert(StringResource.ErrorString, e, StringResource.OkString);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("RegisterPage");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void AvatarControl_OnAvatarSelected(object sender, SelectedEventArgs selectedEventArgs)
        {
            if (!string.IsNullOrEmpty(selectedEventArgs.Error))
            {
                DisplayAlert("Image crop error", selectedEventArgs.Error, "Cancel");
                return;
            }
            ViewModel.AvatarPath = selectedEventArgs.Path;
            //ToDo: Upload avatar


            //if (Device.RuntimePlatform == Device.Android)
            //{
            //    //ViewModel.Update();
            //}
        }

        private void AvatarControl_OnAvatarSelecting(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform != Device.Android) return;
            var appSettings = DependencyService.Get<IApplicationParamsProvider>();
            appSettings.IsPickImage = true;
        }

        private async void Register_OnClicked(object sender, object e)
        {
            await ViewModel.Submit();
        }

        private void LoginMode_OnTapped(object sender, EventArgs e)
        {
            ViewModel.SetLoginMode();
        }

        private void RegisterMode_OnTapped(object sender, EventArgs e)
        {
            ViewModel.SetRegisterMode();
        }

        private void ForgotPassword_OnTapped(object sender, EventArgs e)
        {
            ViewModel.SetResetMode();
        }

        private void TopBaner_OnShare(object sender, EventArgs e)
        {
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

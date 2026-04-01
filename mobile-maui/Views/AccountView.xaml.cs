using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class AccountView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private IAccountViewModel ViewModel
        {
            get => (IAccountViewModel)BindingContext;
            set => BindingContext = value;
        }

        public AccountView(IAccountViewModel viewModel) : base(MenuItemsEnum.More)
        {
            logger.Log("AccountPage");
            analyticTracker.SetView("Account Page", nameof(AccountView));
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
            
            //Gender List Binding (cause binding from xaml not supported now)
            ViewModel.GenderList.ForEach(g => GenderPicker.Items.Add(g));
            GenderPicker.SelectedIndex = ViewModel.Gender;
            //EducationLevel binding
            ViewModel.EducationLevelList.ForEach(g => EducationLevelPicker.Items.Add(g));
            EducationLevelPicker.SelectedIndex = ViewModel.EducationLevel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("AccountPage");
        }

        private void AvatarControl_OnAvatarSelected(object sender, SelectedEventArgs selectedEventArgs)
        {
            if (!string.IsNullOrEmpty(selectedEventArgs.Error))
            {
                DisplayAlert("Image crop error", selectedEventArgs.Error, "Cancel");
                return;
            }
            ViewModel.AvatarPath = selectedEventArgs.Path;
            ViewModel.Save();
            if (Device.RuntimePlatform == Device.Android)
            {
                ViewModel.Update();
            }
        }

        private void AvatarControl_OnAvatarSelecting(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform != Device.Android) return;
            var appSettings = DependencyService.Get<IApplicationParamsProvider>();
            appSettings.IsPickImage = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.Save();
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            ViewModel.DateOfBirdth = DateTime.MinValue;
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

        private async void RoundedButton_OnTouched(object sender, object e)
        {
            await Navigation.MoveToDeleteAccount();
        }
    }
}

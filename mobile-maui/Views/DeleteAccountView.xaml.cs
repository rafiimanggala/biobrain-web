using System;
using System.ComponentModel;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeleteAccountView : BaseView
    {
        private IDeleteAccountViewModel ViewModel
        {
            get => (IDeleteAccountViewModel)BindingContext;
            set => BindingContext = value;
        }
        public DeleteAccountView(IDeleteAccountViewModel viewModel) : base(MenuItemsEnum.More)
        {
            InitializeComponent();

            ViewModel = viewModel;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            ViewModel.Error += ViewModelOnError;
            ViewModel.FinishDelete += ViewModelOnFinishDelete;
        }

        protected override void OnDisappearing()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            ViewModel.Error -= ViewModelOnError;
            ViewModel.FinishDelete -= ViewModelOnFinishDelete;
            if (ViewModel.Step != DeleteAccountStage.Result)
                base.OnDisappearing();
            else
                Navigation.MoveToAuthorization();
        }

        private async void ViewModelOnFinishDelete(object sender, EventArgs e)
        {
            await Navigation.MoveToAuthorization();
        }

        private async void ViewModelOnError(object sender, string e)
        {
            await DisplayAlert(StringResource.ErrorString, e, StringResource.OkString);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ReasonControl.IsVisible = false;
        }

        // Work around for radiobuttons
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsReasonVisible")
            {
                ReasonControl.IsVisible = ViewModel.IsReasonVisible;
            }
        }

        private void CancelButton_OnTouched(object sender, object e)
        {
            Navigation.PopAsync(false);
        }

        private void ContinueButton_OnTouched(object sender, object e)
        {
            ViewModel.GoToReason();
        }

        private void Reason_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var radioButton = (RadioButton) sender;
            if(radioButton == null || !e.Value) return;

            ViewModel.Reason = radioButton.Content?.ToString() ?? string.Empty;
        }

        private void FinishButton_OnTouched(object sender, object e)
        {
            ViewModel.Submit();
        }

        private void OkButton_OnTouched(object sender, object e)
        {
            ViewModel.Finish();
        }

        private void RadioButton_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                var radioButton = (RadioButton)sender;
                if (radioButton == null) return;

                radioButton.InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
            }
        }

        // Workaround (navigate home not triggers ondisapearing)
        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            if(e.MenuItem != MenuItemsEnum.Home) return;
            e.Cancel = true;
            if (ViewModel.Step != DeleteAccountStage.Result)
                base.OnDisappearing();
            else
                Navigation.MoveBack();
        }
    }
}
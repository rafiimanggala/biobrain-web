using System;
using BioBrain.Extensions;
using BioBrain.ViewModels.Interfaces;
using Common.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class PurchasesPage : ContentPage
    {
        IErrorLog loger = DependencyService.Get<IErrorLog>();

        private IPurchasesViewModel ViewModel
        {
            get { return (IPurchasesViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        public PurchasesPage(IPurchasesViewModel viewModel)
        {
            loger.Log("PurchasesView");
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.GetData();
        }

        private async void HomeButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.MoveToHome();
        }

        private void BackButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.MoveBack();
        }

        private void Purchase_OnClicked(object sender, object e)
        {
        }

        private void FakeTap(object sender, EventArgs e)
        {
            //For ios
        }
    }
}

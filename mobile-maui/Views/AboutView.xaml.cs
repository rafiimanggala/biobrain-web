using System;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class AboutView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        public IAboutViewModel ViewModel
        {
            get => (IAboutViewModel)BindingContext;
            set => BindingContext = value;
        }

        public AboutView(IAboutViewModel viewModel) : base(MenuItemsEnum.More)
        {
            logger.Log("AboutView");
            analyticTracker.SetView("About Biobrain", nameof(AboutView));
            InitializeComponent();
            ViewModel = viewModel;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.GetData();
            logger.Log("AboutView");
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

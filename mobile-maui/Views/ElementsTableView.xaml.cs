using System;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ElementsTableView : IBaseView
    {
        public MenuItemsEnum MenuItem { get; } = MenuItemsEnum.PeriodicTable;
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly StackOrientation startOrientation;
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public IElemetsTablePageViewModel ViewModel
        {
            get => (IElemetsTablePageViewModel)BindingContext;
            set => BindingContext = value;
        }

        public ElementsTableView(IElemetsTablePageViewModel viewModel)
        {
            logger.Log("Periodic table");
            analyticTracker.SetView("Elements Table Page", nameof(ElementsTableView));
            InitializeComponent();
            ViewModel = viewModel;
            startOrientation = GetOrientation();

            //For ios need to call one time before OnAppearing
            if (Device.RuntimePlatform == Device.iOS)
            {
                DependencyService.Get<IOrientationService>().Landscape();
            }

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            logger.Log("Periodic table - Appearing");
            base.OnAppearing();

            //For android need to lock orientation when OnAppearing and unlock when OnDisappearing
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IOrientationService>().Landscape();
        }

        private StackOrientation GetOrientation()
        {
            return Application.Current.MainPage.Height >= Application.Current.MainPage.Width ? StackOrientation.Vertical : StackOrientation.Horizontal;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (Device.RuntimePlatform == Device.Android)
            {
                if (startOrientation == StackOrientation.Vertical)
                    DependencyService.Get<IOrientationService>().Portrait();
                else
                    DependencyService.Get<IOrientationService>().All();
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (startOrientation == StackOrientation.Vertical)
                    DependencyService.Get<IOrientationService>().Portrait();
                else
                    DependencyService.Get<IOrientationService>().All();
            }
        }

        private void BackButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(false);
        }
    }
}
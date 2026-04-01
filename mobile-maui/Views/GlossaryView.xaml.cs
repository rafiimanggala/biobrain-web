using System;
using System.Threading.Tasks;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Unity;
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class GlossaryView : BasePurchaseView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        private IGlossaryViewModel ViewModel
        {
            get => (IGlossaryViewModel)BindingContext;
            set => BindingContext = value;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("GlossaryView");
        }

        public GlossaryView(IGlossaryViewModel viewModel) : base(MenuItemsEnum.Glossary)
        {
            logger.Log("GlossaryView");
            analyticTracker.SetView("Glossary Page", nameof(GlossaryView));
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GlossaryAlphabetControl_OnTouched(object sender, AlphabetControlEventArgs e)
        {
            SharePopup.ClosePopup();
            if (Popup.IsVisible) return;
            if (ViewModel.CanNavigate(e.Letter.ToString()))
                MoveToLetter(e.Letter);
            else
                await Alert();
        }

        //private async Task Alert()
        //{
        //    var price = await ViewModel.GetPrice();
        //    var result = await DisplayAlert(StringResource.LiteVersionHeader, string.Format(StringResource.GetFullVersionString, price?.LocalizedPrice ?? ""), StringResource.GetFullString, StringResource.CancelString);
        //    if (result)
        //    {
        //        if (await ViewModel.MakePurchase())
        //            await Navigation.MoveToAreasAndInitUpdate();
        //    }
        //}

        private async void MoveToLetter(char letter)
        {
            await Navigation.PushAsync(App.Container.Resolve<LetterView>(new ParameterOverride("letter", letter)), false);
        }

        private async void SearchBar_OnOnSearch(object sender, SearchEventArgs e)
        {
            SharePopup.ClosePopup();
            var id = ViewModel.GetWordID(e.SearchText);
            if (id < 0)
            {
                ShowPopup();
            }
            else
            {
                analyticTracker.SendEvent(AnalyticEvents.GlossaryViewSearched, AnalyticEventParams.SearchText, e.SearchText);
                await Navigation.MoveToWord(id);
            }
        }

        private void ShowPopup()
        {
            if (Popup.IsVisible) return;
            MainStack.IsEnabled = false;
            Popup.InitPopup(PopupStyles.Message);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Popup.Margin = height > width ? new Thickness(15, 180) : new Thickness(10, 10);
        }

        private void ShareButton_OnTapped(object sender, EventArgs e)
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

        private void Popup_OnClosed(object sender, PopupCloseEventArgs e)
        {
            MainStack.IsEnabled = true;
        }

        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            SharePopup.ClosePopup();
            if (Popup.IsVisible) e.Cancel = true;
        }
    }
}

using System;
using System.Linq;
using BioBrain.Helpers;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class WordView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private IWordViewModel ViewModel
        {
            get => (IWordViewModel)BindingContext;
            set => BindingContext = value;
        }

        public WordView(IWordViewModel viewModel) : base(MenuItemsEnum.Glossary)
        {
            analyticTracker.SetView("Glossary Word Page", nameof(WordView));
            InitializeComponent();
            ViewModel = viewModel;
            isPostback = false;
            Popup.Closed += PopupOnClosed;
        }

        private void PopupOnClosed(object sender, PopupCloseEventArgs e)
        {
            MainStack.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                logger.Log($"WordView word:{ViewModel.Term}");
            }
            catch (Exception ex)
            {
                logger.Log($"WordView OnAppearing error: {ex.Message}");
            }
        }

        private bool isPostback;

        private void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (Popup.IsVisible)
            {
                e.Cancel = true;
                return;
            }
            if (isPostback)
            {
                var response = ResponseHelper.GetResponseForMaterials(e.Url);
                if (response.Type != ResponseTypes.Glossary)
                {
                    e.Cancel = true;
                    return;
                }
                
                var glossaryEntry = ViewModel.GetGlossaryTerm(response.ParsedResponse.Values.FirstOrDefault());
                if (glossaryEntry == null)
                {
                    e.Cancel = true;
                    return;
                }

                Popup.WebSource = new UrlWebViewSource { Url = glossaryEntry.PopupFilePath };
                InitPopup(PopupStyles.Glossary);

                e.Cancel = true;
            }
            else
                isPostback = true;
        }

        private void PopupWebView_OnContentChaging(object sender, PopupContentChangigEventArgs e)
        {
            var responce = ResponseHelper.GetResponseForMaterials(e.Results);
            if (responce.Type != ResponseTypes.Glossary) return;

            var glossaryEntry = ViewModel.GetGlossaryTerm(responce.ParsedResponse.Values.FirstOrDefault());
            if (glossaryEntry == null) return;

            Popup.WebSource = new UrlWebViewSource { Url = glossaryEntry.PopupFilePath };
            InitPopup(PopupStyles.Glossary);
        }

        private void InitPopup(PopupStyles style)
        {
            Popup.IsCorrect = false;
            MainStack.IsEnabled = false;
            Popup.InitPopup(style, null);
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

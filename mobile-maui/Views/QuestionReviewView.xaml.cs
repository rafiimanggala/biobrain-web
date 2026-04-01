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
    public partial class QuestionReviewView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        private IQuestionReviewViewModel ViewModel
        {
            get => (IQuestionReviewViewModel)BindingContext;
            set => BindingContext = value;
        }

        public QuestionReviewView(IQuestionReviewViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            logger.Log("QuestionReviewView");
            analyticTracker.SetView("Question Review Page", nameof(QuestionReviewView));
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("QuestionReviewView");
        }

        private bool isPostback;

        private void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (Popup.IsVisible)
            {
                e.Cancel = true;
                return;
            }
            if (isPostback || Device.RuntimePlatform == Device.Android)
            {
                var responce = ResponseHelper.GetResponseForMaterials(e.Url);
                if (responce.Type != ResponseTypes.Glossary) return;
                var glossaryEntry = ViewModel.GetGlossryTerm(responce.ParsedResponse.Values.FirstOrDefault());
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

        private void InitPopup(PopupStyles style, AnswerResultViewModel answer = null)
        {
            Popup.IsCorrect = answer?.IsCorrect ?? false;
            MainStack.IsEnabled = false;
            Popup.InitPopup(style, answer);
        }

        private void PopupWebView_OnContentChaging(object sender, PopupContentChangigEventArgs e)
        {
            var responce = ResponseHelper.GetResponseForMaterials(e.Results);
            if (responce.Type != ResponseTypes.Glossary)
            {
                return;
            }
            var glossaryEntry = ViewModel.GetGlossryTerm(responce.ParsedResponse.Values.FirstOrDefault());
            if (glossaryEntry == null)
            {
                return;
            }
            Popup.WebSource = new UrlWebViewSource { Url = glossaryEntry.PopupFilePath };
            InitPopup(PopupStyles.Glossary);
        }

        private void Popup_OnClosed(object sender, PopupCloseEventArgs e)
        {
            Popup.IsVisible = false;
            MainStack.IsEnabled = true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Popup.Margin = height > width ? new Thickness(5, 170) : new Thickness(10, 10);
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

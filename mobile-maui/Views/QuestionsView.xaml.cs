using System;
using System.Collections.Generic;
using System.Linq;
using BioBrain.Extensions;
using BioBrain.Helpers;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace BioBrain.Views
{
    public partial class QuestionsView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public IQuestionsViewModel ViewModel
        {
            get => (IQuestionsViewModel)BindingContext;
            set => BindingContext = value;
        }

        public QuestionsView(IQuestionsViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            try
            {
                logger.Log($"QuestionView T:{viewModel.TopicName} M:{viewModel.MaterialID} Q:{viewModel.CurrentQuestion?.QuestionOrder}");
            }
            catch (Exception)
            {
                logger.Log($"QuestionView T:{viewModel.TopicName} M:{viewModel.MaterialID} Q:{viewModel.CurrentQuestionIndex} (can't get question number)");
            }

            IsQuestionView = true;
            analyticTracker.SetView("Quiz Page", nameof(QuestionsView));
            InitializeComponent();

            ViewModel = viewModel;
            ViewModel.Error += ViewModelOnError;
            ViewModel.AuthError += ViewModelOnAuthError;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ViewModelOnAuthError(object sender, string s)
        {
            //if (await DisplayAlert(StringResource.EmailString, StringResource.SelectEmailString,
            //              StringResource.AddEmailString, StringResource.CancelString))
            //    await ViewModel.GetEmailAndRetry();
            await Navigation.MoveToAuthorization();
        }

        private async void ViewModelOnError(object sender, string s)
        {
            await DisplayAlert(StringResource.ErrorString, s, StringResource.OkString);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                ViewModel.GetData();
                SetData();
                Log();
                analyticTracker.SendEvent(AnalyticEvents.QuizStart, new Dictionary<string, string>
                {
                    {AnalyticEventParams.Area, ViewModel.AreaName},
                    {AnalyticEventParams.Topic, ViewModel.TopicName},
                    {AnalyticEventParams.Level, ViewModel.LevelName},
                });
            }
            catch (Exception ex)
            {
                logger.Log($"QuestionsView OnAppearing error: {ex.Message}");
            }
        }

        private void Log()
        {
            logger.Log($"QuestionView T:{ViewModel.TopicName} M:{ViewModel.MaterialID} Q:{ViewModel.CurrentQuestion?.QuestionOrder}");
        }

        private void SetData()
        {
            //QuestionWebView.Source = new UrlWebViewSource { Url = ViewModel.CurrentQuestion.FilePath };
            HintButton.IsVisible = ViewModel.CurrentQuestion.IsHintEnabled;
        }

        private bool isPostback;
        private bool isReload;

        private void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (isReload)
            {
                isPostback = true;
                isReload = false;
                return;
            }
            if (Popup.IsVisible)
            {
                e.Cancel = true;
                return;
            }
            if (isPostback)
            {
                var response = ResponseHelper.GetResponseForMaterials(e.Url);
                if (response.Type == ResponseTypes.Error)
                {
                    isPostback = false;
                    isReload = true;
                    QuestionWebView.Eval("location.reload();");
                    return;
                }

                if (response.Type != ResponseTypes.Glossary)
                {
                    if (ViewModel.CurrentQuestionIndex > ViewModel.Questions.Count - 1)
                    {
                        e.Cancel = true;
                        return;
                    }
                    var answer = ViewModel.CurrentQuestion.CheckAnswer(response);
                    Popup.WebSource = new UrlWebViewSource { Url = answer.FilePath };
                    InitPopup(PopupStyles.Answer, answer);
                }
                else
                {
                    var glossaryEntry = ViewModel.GetGlossryTerm(response.ParsedResponse.Values.FirstOrDefault());
                    if (glossaryEntry == null)
                    {
                        e.Cancel = true;
                        return;
                    }

                    Popup.WebSource = new UrlWebViewSource { Url = glossaryEntry.PopupFilePath };
                    InitPopup(PopupStyles.Glossary);
                }

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

        private async void NavigateToResultPage(int materialID)
        {
            await Navigation.MoveToResults(materialID);
        }

        private void OnHintTapped(object sender, EventArgs e)
        {
            if (Popup.IsVisible) return;
            var path = FileHelper.WriteFile(ViewModel.CurrentQuestion.Hint, FileTypes.Answer);
            Popup.WebSource = new UrlWebViewSource { Url = path };
            InitPopup(PopupStyles.Hint);
        }

        private void PopupWebView_OnContentChaging(object sender, PopupContentChangigEventArgs e)
        {
            var response = ResponseHelper.GetResponseForMaterials(e.Results);
            if (response.Type != ResponseTypes.Glossary) return;

            var glossaryEntry = ViewModel.GetGlossryTerm(response.ParsedResponse.Values.FirstOrDefault());
            if (glossaryEntry == null) return;

            Popup.WebSource = new UrlWebViewSource { Url = glossaryEntry.PopupFilePath };
            InitPopup(PopupStyles.Glossary);
        }

        private async void Popup_OnClosed(object sender, PopupCloseEventArgs e)
        {
            var answer = (AnswerResultViewModel)e.Results;
            if (answer != null && (answer.IsSecondTry || answer.IsCorrect))
            {
                isPostback = false;
                ViewModel.ToNextQuestion();

                if (Device.RuntimePlatform == Device.Android)
                    QuestionWebView.Reload();

                if (ViewModel.CurrentQuestionIndex > ViewModel.Questions.Count - 1)
                {
                    ViewModel.SetMaterialDone();
                    //if (Settings.IsDemo)
                    //{
	                   // if (!CrossConnectivity.Current.IsConnected)
	                   // {
		                  //  OnError(this, StringResource.NoInternetError);
		                  //  return;
	                   // }
                    //    await Alert();
                    //    await Navigation.MoveBack();
                    //}
                    //else
                        NavigateToResultPage(ViewModel.MaterialID);

                    MainStack.IsEnabled = true;
                    return;
                }
                SetData();
                Log();
            }
            MainStack.IsEnabled = true;
        }

        private double previousWidth;
        private double previousHeight;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width == previousWidth && height == previousHeight) return;
            Popup.Margin = height > width ? new Thickness(5, 170, 5, 80) : new Thickness(10, 10);
            if (Device.RuntimePlatform == Device.iOS)
            {
                isReload = true;
                QuestionWebView.Eval("location.reload();");
            }
            previousWidth = width;
            previousHeight = height;
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

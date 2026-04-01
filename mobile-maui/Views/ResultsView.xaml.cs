using System;
using System.Collections.Generic;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.LayoutControls;
using Unity;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class ResultsView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public IResultViewModel ViewModel
        {
            get => (IResultViewModel)BindingContext;
            set => BindingContext = value;
        }

        public ResultsView(IResultViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            logger.Log("ResultsView");
            analyticTracker.SetView("Quiz Result Page", nameof(ResultsView));
            InitializeComponent();
            ViewModel = viewModel;
        }

        private bool isPostBack;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            logger.Log("ResultsView");
            try
            {
                analyticTracker.SendEvent(AnalyticEvents.QuizEnd, new Dictionary<string, string>
                {
                    { AnalyticEventParams.Area, ViewModel.AreaName},
                    { AnalyticEventParams.Topic, ViewModel.TopicName},
                    { AnalyticEventParams.Level, ViewModel.LevelName},
                    { AnalyticEventParams.QuizEndResult, $"{ViewModel.Percent}%"},
                });

                if (isPostBack) return;
                if (ViewModel.IsRateNeed())
                {
                    LikePopup.Opacity = 1;
                }
                else
                {
                    LikePopup.IsVisible = false;
                }
                isPostBack = true;
            }
            catch (Exception ex)
            {
                logger.Log($"ResultsView OnAppearing error: {ex.Message}");
            }
        }

        private void Button_OnClicked(object parameter, object o)
        {
            Popup.ClosePopup();
            NavigateToMaterial(ViewModel.NextTopicMaterialID);
        }

        private async void NavigateToTopic()
        {
            Navigation.DeleteOfType(typeof(QuestionsView));
            Navigation.DeleteOfType(typeof(MaterialView));
            await Navigation.MoveBack();
            GC.Collect();
        }

        private async void NavigateToMaterial(int materialID)
        {
	        if (materialID < 0)
	        {
		        NavigateToTopic();
		        return;
	        }
            Navigation.DeleteOfType(typeof(QuestionsView));
            Navigation.DeleteOfType(typeof(MaterialView));
            await Navigation.GoToMaterial(materialID);
            Navigation.DeleteOfType(typeof(ResultsView));
        }

        private async void NavigateToReview(int questionID)
        {
            await Navigation.MoveToQuestionReview(questionID);
        }

        private void QuestionsList_OnTouched(object sender, HexagonResultControlEventArgs e)
        {
            Popup.ClosePopup();
            NavigateToReview(e.SelectedElementID);
        }

        private void NextButton_OnClicked(object sender, object e)
        {
            Popup.ClosePopup();
            if (ViewModel.NextMaterialID < 0)
            {
                NavigateToMaterial(ViewModel.NextTopicMaterialID);
                return;
            }
            NavigateToMaterial(ViewModel.NextMaterialID);
        }

        private async void Yes_OnClicked(object sender, object e)
        {
            LikePopup.IsVisible = false;
            //If like
            var rater = DependencyService.Get<IRateApp>();
            var result = await rater.RateApp();
            ViewModel.SaveRateResult(result);
        }

        private async void No_OnClicked(object sender, object e)
        {
            LikePopup.IsVisible = false;
            await Navigation.PushAsync(App.Container.Resolve<SendReviewPage>(), false);
        }

        private void LikeTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            LikePopup.IsVisible = false;
            ViewModel.SaveRateResult(RateResult.NextTime);
        }

        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            Popup.ClosePopup();
            if (e.MenuItem == MenuItemsEnum.Back)
            {
                Navigation.DeleteOfType(typeof(QuestionsView));
                GC.Collect();
            }
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

        protected override bool OnBackButtonPressed()
        {
            Navigation.DeleteOfType(typeof(QuestionsView));
            return base.OnBackButtonPressed();
        }
    }
}

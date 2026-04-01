using System;
using System.Collections.Generic;
using System.Linq;
using BioBrain.Helpers;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls.Buttons;
using CustomControls.LayoutControls;
using Unity.Resolution;
using Microsoft.Maui.Controls;
using Unity;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using Microsoft.Maui.Networking;

namespace BioBrain.Views
{
    public partial class MaterialView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        public IMaterialsViewModel ViewModel
        {
            get => (IMaterialsViewModel) BindingContext;
            set => BindingContext = value;
        }

        public MaterialView(IMaterialsViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            logger.Log("MaterialView T:" + viewModel.TopicName);
            analyticTracker.SetView("Material Page", nameof(MaterialView));
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.Error +=ViewModelOnError;
            ViewModel.AuthError += ViewModelOnAuthError;
            LogMaterialWatched();
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
                logger.Log("MaterialView T:"+ViewModel.TopicName);
                ViewModel.SetQuizButtonText();
            }
            catch (Exception ex)
            {
                logger.Log($"MaterialView OnAppearing error: {ex.Message}");
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel.SetQuizButtonText();
        }

        private async void GoToMaterial(int id)
        {
            if (!MainStack.IsEnabled) return;
            if (id < 0)
            {
	            if (Settings.IsDemo)
	            {
		            if (!CrossConnectivity.Current.IsConnected)
		            {
			            OnError(this, StringResource.NoInternetError);
			            return;
		            }

		            await Alert();
		            return;
	            }
	            else
	            {
		            await DisplayAlert(StringResource.Level, StringResource.ComingSoonLevel, StringResource.OkString);
                    return;
	            }
            }

            isPostBack = false;
            MainStack.IsEnabled = false;
            ViewModel = App.Container.Resolve<IMaterialsViewModel>(new ParameterOverride("materialID", id));
            ViewModel.PropertiesChanged();
            LogMaterialWatched();
            MainStack.IsEnabled = true;
        }

        private void LogMaterialWatched()
        {
            analyticTracker.SendEvent(AnalyticEvents.MaterialVisited, new Dictionary<string, string>
            {
                { AnalyticEventParams.Area, ViewModel.AreaName },
                { AnalyticEventParams.Topic, ViewModel.TopicName },
                { AnalyticEventParams.Level, ViewModel.LevelName }
            });
        }
        
        private async void GoToQuestions(int topicID, int materialID)
        {
            if (!ViewModel.IsHaveQuestions)
            {
                await DisplayAlert(StringResource.QuestionString, StringResource.NoQuestionString,
                    StringResource.OkString);
                return;
            }
            
            await Navigation.MoveToQuestions(topicID, materialID);
        }

        private bool isPostBack;

        private void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (Popup.IsVisible)
            {
                e.Cancel = true;
                return;
            }
            if (isPostBack)
            {
                e.Cancel = true;
                var response = ResponseHelper.GetResponseForMaterials(e.Url);
                if (response.Type == ResponseTypes.Material) HandleMaterialLink(response);
                if (response.Type == ResponseTypes.Area) HandleAreaLink(response);
                if (response.Type != ResponseTypes.Glossary) return;

                var glossaryEntry = ViewModel.GetGlossryTerm(response.ParsedResponse.Values.FirstOrDefault());
                if (glossaryEntry == null) return;

                Popup.WebSource = new UrlWebViewSource {Url = glossaryEntry.PopupFilePath};
                MainStack.IsEnabled = false;
                Popup.InitPopup(PopupStyles.Glossary);
            }
            else
                isPostBack = true;
        }

        private async void HandleMaterialLink(WebViewResponseModel response)
        {
            var topicIdRef = response.ParsedResponse["topic"];
            var levelIdRef = response.ParsedResponse["level"];
            if (!int.TryParse(topicIdRef, out var topicId)) return;
            if(!int.TryParse(levelIdRef, out var levelId)) return;

            var materialId = ViewModel.GetMaterial(topicId, levelId);
            if(materialId < 1) return;
            await Navigation.GoToMaterial(materialId);
        }

        private async void HandleAreaLink(WebViewResponseModel response)
        {
            var areaIdRef = response.ParsedResponse["area"];
            if (!int.TryParse(areaIdRef, out var areaId)) return;

            await Navigation.MoveToTopics(areaId);
        }

        private void LevelsBar_OnTouched(object sender, LevelsBar.LevelsBarEventArgs e)
        {
            GoToMaterial(e.Key);
            ViewModel.SetQuizButtonText();
        }

        private void PopupWebView_OnContentChaging(object sender, PopupContentChangigEventArgs e)
        {
            var response = ResponseHelper.GetResponseForMaterials(e.Results);
            if (response.Type != ResponseTypes.Glossary) return;

            var glossaryEntry = ViewModel.GetGlossryTerm(response.ParsedResponse.Values.FirstOrDefault());
            if (glossaryEntry == null) return;

            Popup.WebSource = new UrlWebViewSource {Url = glossaryEntry.PopupFilePath};
            MainStack.IsEnabled = false;
            Popup.InitPopup(PopupStyles.Glossary);
        }

        private void Popup_OnClosed(object sender, PopupCloseEventArgs e)
        {
            MainStack.IsEnabled = true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Popup.Margin = height>width ? new Thickness(5, 150) : new Thickness(10, 10);
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

        private void RoundedButton_OnTouched(object sender, object e)
        {
            GoToQuestions(ViewModel.TopicID, ViewModel.MaterialID);
        }
    }
}

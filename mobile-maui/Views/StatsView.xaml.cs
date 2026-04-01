using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Unity;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsView
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private IStatsViewModel ViewModel
        {
            get => (IStatsViewModel) BindingContext;
            set => BindingContext = value;
        }

        public StatsView(IStatsViewModel viewModel) : base(MenuItemsEnum.Stats)
        {
            logger.Log("StatsView");
            analyticTracker.SetView("Stats Page", nameof(StatsListView));
            InitializeComponent();
            ViewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            logger.Log("StatsView");
            try
            {
                ViewModel.GetData();
            }
            catch (Exception ex)
            {
                logger.Log($"StatsView OnAppearing error: {ex.Message}");
            }
            base.OnAppearing();
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

        private async void More_OnClicked(object sender, object e)
        {
            await Navigation.PushAsync(App.Container.Resolve<StatsListView>(), false);
        }

        private async void Retake_OnTapped(object sender, object e)
        {
            var frame = (BindableObject) sender;
            var statEntry = (IStatEntryViewModel) frame?.BindingContext;
            if(statEntry == null) return;

            analyticTracker.SendEvent(AnalyticEvents.RetakeQuizTapped);

            await Navigation.MoveToQuestions(statEntry.TopicId, statEntry.MaterialId);
        }

        private async void StudyButton_OnTouched(object sender, object e)
        {
            var frame = (BindableObject)sender;
            var statEntry = (IStatEntryViewModel)frame?.BindingContext;
            if (statEntry == null) return;

            analyticTracker.SendEvent(AnalyticEvents.StudyMoreTapped);

            await Navigation.GoToMaterial(statEntry.MaterialId);
        }

        private void TopBaner_OnSend(object sender, EventArgs e)
        {
            ViewModel.ToggleSendMode();
        }
    }
}
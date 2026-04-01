using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatsListView 
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        private IStatListViewModel ViewModel
        {
            get => (IStatListViewModel)BindingContext;
            set => BindingContext = value;
        }

        public StatsListView (IStatListViewModel viewModel) : base(MenuItemsEnum.Stats)
		{
		    logger.Log("TopicsView");
            analyticTracker.SetView("Stats List Page", nameof(StatsListView));
            InitializeComponent();
		    ViewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                ViewModel.GetData();
            }
            catch (Exception ex)
            {
                logger.Log($"StatsListView OnAppearing error: {ex.Message}");
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

        private void TopBaner_OnSend(object sender, EventArgs e)
        {
            ViewModel.ToggleSendMode();
        }

        private async void StudyButton_OnTouched(object sender, object e)
        {
            var frame = (BindableObject)sender;
            var statEntry = (IStatEntryViewModel)frame?.BindingContext;
            if (statEntry == null) return;

            analyticTracker.SendEvent(AnalyticEvents.StudyMoreTapped);

            await Navigation.GoToMaterial(statEntry.MaterialId);
        }

        private async void Retake_OnTapped(object sender, object e)
        {
            var frame = (BindableObject)sender;
            var statEntry = (IStatEntryViewModel)frame?.BindingContext;
            if (statEntry == null) return;

            analyticTracker.SendEvent(AnalyticEvents.RetakeQuizTapped);

            await Navigation.MoveToQuestions(statEntry.TopicId, statEntry.MaterialId);
        }
    }
}
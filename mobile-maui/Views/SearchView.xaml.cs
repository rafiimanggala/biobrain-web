using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using CustomControls.LayoutControls;
using Unity;
using Unity.Resolution;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchView
    {
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public ISearchViewModel ViewModel
        {
            get => (ISearchViewModel) BindingContext;
            set => BindingContext = value;
        }

        public SearchView(ISearchViewModel viewModel) : base(MenuItemsEnum.Home)
        {
            analyticTracker.SetView("Search Page", nameof(SearchView));
            InitializeComponent();
            ViewModel = viewModel;

            ViewModel.NavigateToArea += ViewModelOnNavigateToArea;
            ViewModel.NavigateToGlossary += ViewModelOnNavigateToGlossary;
            ViewModel.NavigateToMaterial += ViewModelOnNavigateToMaterial;
            ViewModel.NavigateToTopic += ViewModelOnNavigateToTopic;

            ResultsListView.ItemSelected += (sender, args) => ResultsListView.SelectedItem = null;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ViewModelOnNavigateToTopic(object sender, int e)
        {
            await Navigation.GoToMaterial(e);
        }

        private async void ViewModelOnNavigateToMaterial(object sender, int e)
        {
            await Navigation.GoToMaterial(e);
        }

        private async void ViewModelOnNavigateToGlossary(object sender, int e)
        {
            await Navigation.MoveToWord(e);
        }

        private async void ViewModelOnNavigateToArea(object sender, int e)
        {
            await Navigation.PushAsync(App.Container.Resolve<TopicsView>(string.Empty, new ParameterOverride("areaOfStudyId", e)), false);
        }

        private void SearchBar_OnOnSearch(object sender, SearchEventArgs e)
        {
            ViewModel.Search(e.SearchText);
        }

        private void Element_OnTapped(object sender, EventArgs e)
        {
            var elementviewModel = (ISearchResultViewModel)((BindableObject) sender)?.BindingContext;
            if(elementviewModel == null) return;
            ViewModel.HandleElementClick(elementviewModel);
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
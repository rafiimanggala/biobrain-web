using System;
using System.Collections.ObjectModel;
using BioBrain.ViewModels.Implementation;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ISearchViewModel
    {
        event EventHandler<int> NavigateToGlossary;
        event EventHandler<int> NavigateToArea;
        event EventHandler<int> NavigateToTopic;
        event EventHandler<int> NavigateToMaterial;
        ObservableCollection<ISearchResultViewModel> SearchResult { get; }

        void Search(string text);
        void HandleElementClick(ISearchResultViewModel viewModel);
    }
}
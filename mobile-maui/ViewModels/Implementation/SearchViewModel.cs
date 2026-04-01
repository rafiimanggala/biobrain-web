using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BioBrain.Annotations;
using BioBrain.Extensions;
using BioBrain.Factories;
using BioBrain.Helpers;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;
// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

namespace BioBrain.ViewModels.Implementation
{
    public class SearchViewModel : ISearchViewModel, INotifyPropertyChanged
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IMaterialsRepository materialsRepository;
        private readonly IAreasRepository areasRepository;
        private readonly ITopicsRepository topicsRepository;
        private readonly IGlossaryRepository glossaryRepository;
        private readonly ILevelTypesRepository levelTypesRepository;

        //regular expression to remove HTML tags
        private readonly Regex spacesRegEx = new Regex(@"\s+");
        private const int ContextSize = 30;

        public SearchViewModel(string text, IMaterialsRepository materialsRepository, IAreasRepository areasRepository,
            ITopicsRepository topicsRepository, IGlossaryRepository glossaryRepository, ILevelTypesRepository levelTypesRepository)
        {
            this.materialsRepository = materialsRepository;
            this.areasRepository = areasRepository;
            this.topicsRepository = topicsRepository;
            this.glossaryRepository = glossaryRepository;
            this.levelTypesRepository = levelTypesRepository;

            SearchText = text;
            try { Search(text); }
            catch (Exception ex) { logger.Log($"SearchViewModel constructor error: {ex.Message}"); }
        }

        public event EventHandler<int> NavigateToGlossary;
        public event EventHandler<int> NavigateToArea;
        public event EventHandler<int> NavigateToTopic;
        public event EventHandler<int> NavigateToMaterial;

        public ObservableCollection<ISearchResultViewModel> SearchResult { get; } =
            new ObservableCollection<ISearchResultViewModel>();

        public string SearchText { get; set; }

        public void Search(string text)
        {
            SearchResult.Clear();
            if(text.Length < 2) return;

            GlossarySearchInternal(text);
            MaterialsSearchInternal(text);
            OnPropertyChanged(nameof(SearchResult));
        }

        private void GlossarySearchInternal(string text)
        {
            var terms = glossaryRepository.GetByText(text);
            terms.ForEach(t => SearchResult.Add(SearchResultFactory.GetSearchResult($"Glossary - {(string.IsNullOrEmpty(t.Header) ? t.Term : t.Header)}",
                string.Empty,
                SearchResultTypes.Glossary,
                termId: t.TermID)));
        }

        private void MaterialsSearchInternal(string text)
        {
            var result = new List<ISearchResultViewModel>();
            var defaultLevelId = levelTypesRepository.FirstLevelID;
            var areas = areasRepository.GetAll();
            try
            {
                foreach (var area in areas)
                {
                    if (area.AreaName.ToLower().IndexOf(text.ToLower(), StringComparison.Ordinal) >= 0)
                        result.Add(SearchResultFactory.GetSearchResult(area.AreaName, string.Empty,
                            SearchResultTypes.Area,
                            area.AreaID));

                    var topics = topicsRepository.GetForArea(area.AreaID);

                    foreach (var topic in topics)
                    {
                        if (topic.TopicName.ToLower().IndexOf(text.ToLower(), StringComparison.Ordinal) >= 0)
                            result.Add(SearchResultFactory.GetSearchResult($"{area.AreaName} - {topic.TopicName}",
                                string.Empty,
                                SearchResultTypes.Topic,
                                area.AreaID,
                                topic.TopicID,
                                materialsRepository.GetForLevelAndTopic(defaultLevelId, topic.TopicID).MaterialID));

                        var materials = materialsRepository.GetForTopic(topic.TopicID);
                        foreach (var material in materials)
                        {
                            var htmlToFind = material.Text;

                            //suppress HTML, remove multi spaces and newlines
                            var rawText = htmlToFind.RemoveTags();
                            rawText = spacesRegEx.Replace(rawText, " ");
                            //rawText = newLinesRegEx.Replace(rawText, string.Empty);

                            //Search for the text
                            var searchedIndex = rawText.ToLower().IndexOf(text.ToLower(), StringComparison.Ordinal);

                            if (searchedIndex >= 0) //found!
                            {
                                var level = levelTypesRepository.GetByID(material.LevelTypeID);
                                result.Add(SearchResultFactory.GetSearchResult(
                                    $"{area.AreaName} - {topic.TopicName} - {level.LevelName}",
                                    StringsHelper.GetContext(rawText, searchedIndex, text.Length, ContextSize),
                                    SearchResultTypes.Materials,
                                    area.AreaID,
                                    topic.TopicID,
                                    material.MaterialID,
                                    material.LevelTypeID));
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            result.OrderBy(r => r.Type).ForEach(r => SearchResult.Add(r));
        }

        public void HandleElementClick(ISearchResultViewModel viewModel)
        {
            switch (viewModel.Type)
            {
                case SearchResultTypes.Glossary:
                    OnNavigateToGlossary(viewModel.WordId);
                    break;
                case SearchResultTypes.Area:
                    OnNavigateToArea(viewModel.AreaId);
                    break;
                case SearchResultTypes.Topic:
                    OnNavigateToTopic(viewModel.MaterialId);
                    break;
                case SearchResultTypes.Materials:
                    OnNavigateToMaterial(viewModel.MaterialId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnNavigateToGlossary(int e)
        {
            NavigateToGlossary?.Invoke(this, e);
        }

        protected virtual void OnNavigateToArea(int e)
        {
            NavigateToArea?.Invoke(this, e);
        }

        protected virtual void OnNavigateToTopic(int materialId)
        {
            NavigateToTopic?.Invoke(this, materialId);
        }

        protected virtual void OnNavigateToMaterial(int materialId)
        {
            NavigateToMaterial?.Invoke(this, materialId);
        }
    }
}
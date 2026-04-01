using System;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Unity;

namespace BioBrain.Factories
{
    public static class SearchResultFactory
    {
        public static ISearchResultViewModel GetSearchResult(string text, string context, SearchResultTypes type, int areaId = -1, int topicId = -1, int materialId = -1, int levelId = -1, int termId = -1 )
        {
            var result = App.Container.Resolve<ISearchResultViewModel>();
            result.Text = text;
            result.Context = context;
            result.Type = type;
            switch (type)
            {
                case SearchResultTypes.Glossary:
                    result.WordId = termId;
                    break;
                case SearchResultTypes.Area:
                    result.AreaId = areaId;
                    break;
                case SearchResultTypes.Topic:
                    result.AreaId = areaId;
                    result.TopicId = topicId;
                    result.MaterialId = materialId;
                    break;
                case SearchResultTypes.Materials:
                    result.AreaId = areaId;
                    result.TopicId = topicId;
                    result.MaterialId = materialId;
                    result.LevelId = levelId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return result;
        }
    }
}
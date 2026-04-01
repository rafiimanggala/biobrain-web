using BioBrain.ViewModels.Interfaces;
using Common.Enums;

namespace BioBrain.ViewModels.Implementation
{
    public class SearchResultViewModel : ISearchResultViewModel
    {
        public SearchResultTypes Type { get; set; }
        public string Text { get; set; }
        public string Context { get; set; }
        public int AreaId { get; set; }
        public int TopicId { get; set; }
        public int MaterialId { get; set; }
        public int LevelId { get; set; }
        public int WordId { get; set; }
        public bool IsContextVisible => !string.IsNullOrEmpty(Context);
    }
}
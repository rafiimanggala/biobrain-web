using Common.Enums;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ISearchResultViewModel
    {
        SearchResultTypes Type { get; set; }
        string Text { get; set; }
        string Context { get; set; }
        int AreaId { get; set; }
        int TopicId { get; set; }
        int MaterialId { get; set; }
        int LevelId { get; set; }
        int WordId { get; set; }
    }
}
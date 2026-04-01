namespace BioBrain.ViewModels.Interfaces
{
    public interface IHistoryEntryViewModel
    {
        string AreaName { get; set; }

        string TopicName { get; set; }

        string LevelName { get; set; }

        string Date { get; set; }

        string ResultScore { get; set; }
    }
}
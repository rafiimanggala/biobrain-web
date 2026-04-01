using BioBrain.ViewModels.Interfaces;

namespace BioBrain.ViewModels.Implementation
{
    public class HistoryEntryViewModel : IHistoryEntryViewModel
    {
        public string AreaName { get; set; }

        public string TopicName { get; set; }

        public string LevelName { get; set; }

        public string Date { get; set; }

        public string ResultScore { get; set; }
    }
}
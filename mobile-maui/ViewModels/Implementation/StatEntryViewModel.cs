using Common.Interfaces;

namespace BioBrain.ViewModels.Implementation
{
    public class StatEntryViewModel : IStatEntryViewModel
    {
        public string Topic { get; set; }
        public string Level { get; set; }
        public string Score { get; set; }
        public string Date { get; set; }
        public int TopicId { get; set; }
        public int MaterialId { get; set; }

        public bool IsNeedToSend { get; set; }
    }
}
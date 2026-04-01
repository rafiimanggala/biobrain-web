using System.Collections.ObjectModel;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ITopicsViewModel : IBasePurchasableViewModel
    {
        ObservableCollection<ITopicViewModel> Topics { get; set; }
        string AreaOfStudyName { get; }
        string AreaOfStudyIcon { get; set; }
        void GetTopics();
    }
}
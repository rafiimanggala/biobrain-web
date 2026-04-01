using System.Collections.Generic;
using System.ComponentModel;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IHistoryViewModel : INotifyPropertyChanged
    {
        List<IHistoryEntryViewModel> Entries { get; set; }

        List<IAreaViewModel> Areas { get; set; }

        List<ITopicViewModel> Topics { get; set; }

        int SelectedAreaID { get; set; }

        int SelectedTopicID { get; set; }

        void FilterElements();
    }
}
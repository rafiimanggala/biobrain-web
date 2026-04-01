using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IAreasOfStudyViewModel
    {
	    event EventHandler GetFullData;
        event EventHandler NoEmailError;
        event EventHandler<string> UpdateNeed;

        ObservableCollection<IAreaViewModel> Areas { get; }
        ObservableCollection<IAreaViewModel> ComingSoonAreas { get; }
        bool IsBusy { get; }
        bool IsShowData { get; }
        bool IsLocked { get; }
        void ForceEndProcess();

        void GetAreas();
        Task<bool> SyncData();

        bool CouldNavigateToArea(int areaId);
        //Task GetEmailAndRetry();
    }
}

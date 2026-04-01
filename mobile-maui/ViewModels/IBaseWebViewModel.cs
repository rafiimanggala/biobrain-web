using System.ComponentModel;

namespace BioBrain.ViewModels
{
	public interface IBaseWebViewModel : INotifyPropertyChanged
	{
		bool IsBusy { get; }

		bool IsShowData { get; }

		bool IsLocked { get; }
		void ForceEndProcess();
	}
}
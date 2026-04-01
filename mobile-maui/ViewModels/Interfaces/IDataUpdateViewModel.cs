using System;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
	public interface IDataUpdateViewModel : IBasePurchasableViewModel
	{
		event EventHandler<string> ApplicationUpdateNeed;
		event EventHandler NoEmailError;
		event EventHandler NavigateHome;

		Task CheckForUpdatesInternal();
		Task DownloadAndUpdate();
	}
}
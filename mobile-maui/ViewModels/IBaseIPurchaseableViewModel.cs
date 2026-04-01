using System;
using System.Threading.Tasks;
using CustomControls.Dialogs;
using DAL.Models.Interfaces;
using Plugin.InAppBilling;

namespace BioBrain.ViewModels
{
    public interface IBasePurchasableViewModel : IBaseWebViewModel
	{
	    bool IsAuthorized { get; }
	    Task<bool> MakePurchase();
	    Task<bool> MakePurchase(SubscriptionDialogResult result);
	    event EventHandler<string> Error;
	    event EventHandler<string> AuthError;
	    //Task GetEmailAndRetry();
	    Task<InAppBillingProduct> GetPrice();
	    Task<InAppBillingProduct> GetMonthPrice();
	    Task<InAppBillingProduct> GetYearPrice();
	    Task<bool> IsPurchased();
	    IAccountModel GetAccount();
	    Task<bool> Authorize();
	    Task SendLogs();
    }
}
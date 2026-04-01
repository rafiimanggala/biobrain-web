using System;
using System.Threading.Tasks;
using BioBrain.Models;
using Common.Enums;
using CustomControls.Dialogs;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using Plugin.InAppBilling;

// Resolve ambiguity between Common.Enums.AppActions and Microsoft.Maui.ApplicationModel.AppActions
using AppActions = Common.Enums.AppActions;

namespace BioBrain.Services.Interfaces
{
    public interface IFirebaseService
    {
        bool IsAuthorized { get; }
        Task<bool> AuthorizeUser();
        Task<bool> Login(string email, string password);
        Task Register(IRegistrationModel model);
        Task Update(IRegistrationModel model);
        bool CheckUserData();
        Task<AppActions> LookForUpdates();
        Task<bool> CheckIsDemo(bool checkOnline = true);
        Task UpdateDemo(IProgress<int> progressCallback);
        Task<CheckResult> UpdateFull(IProgress<int> progressCallback, bool isFromDemo = false);
        Task<bool> MakePurchase();
        Task<bool> PurchaseSubscription(SubscriptionDialogResult subscriptionType);
        IAccountModel GetAccount();
        Task<InAppBillingProduct> GetCost();
        Task<InAppBillingProduct> GetMonthCost();
        Task<InAppBillingProduct> GetYearCost();
        Task<bool> ActivatePromo();
        Task SendLog(string message, string type);
        Task<bool> CheckIsPurchased(bool checkOnline = true);
        Task<AppActions> CheckAppVersion();
        Task<string> GetAppVersionMessage();
        Task<UserFirebaseModel> GetUser();
        Task<string> UploadAvatar(string path);
        Task ResetPassword(string email);
        Task SaveIosPurchaseIfNotExists();
        Task<CheckResult> CheckFiles(AppContentType contentType);
        Task<InAppBillingPurchase> GetSubscriptionFromStore();
        Task DeleteAccount(string reason);
    }
}
using System;
using System.Diagnostics;
using BioBrain.Services.Interfaces;
using Common;
using Common.Enums;
using AppActions = Common.Enums.AppActions;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;
using Timer = System.Threading.Timer;

namespace BioBrain.Services.Implementations
{
    public class BackgroundWorkerService : IBackgroundWorkerService
    {
        private readonly IFirebaseService firebaseService;
        private readonly IAccountRepository accountRepository;
        private Timer updateCheckerTimer;
        private Timer subscriptionUpdaterTimer;
        private readonly IErrorLog logger = DependencyService.Resolve<IErrorLog>();

        public BackgroundWorkerService(IFirebaseService firebaseService, IAccountRepository accountRepository)
        {
            this.firebaseService = firebaseService;
            this.accountRepository = accountRepository;
        }

        public void StartPeriodically()
        {
            updateCheckerTimer = new Timer(CheckUpdate, Application.Current, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(Settings.UpdateCheckPeriodMinutes));
            subscriptionUpdaterTimer = new Timer(UpdateSubscription, Application.Current, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(Settings.UpdateCheckPeriodMinutes));
        }

        private async void UpdateSubscription(object state)
        {
			try
			{
				logger.Log("Background - Update subscription");
				var account = accountRepository.GetAccountModel();
				if(account == null) return;
				if (account.SubscriptionId == PurchaseTypes.FullAppIos.ToString())
				{
					logger.Log("Background - Full App - return");
					return;
				}

				var subscription = await firebaseService.GetSubscriptionFromStore();
				if (subscription == null || string.IsNullOrEmpty(subscription.ProductId))
				{
					logger.Log("Background - Can't get subscription - return");
					return;
				}

				// Update subscription data
				account.SubscriptionDate = subscription.TransactionDateUtc;
				account.SubscriptionId = subscription.ProductId;
				accountRepository.Update(account);
				logger.Log($"Background - Update subscription ({subscription.ProductId} - {subscription.TransactionDateUtc})");
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				logger.Log("Error update subscription in background");
			}
		}

        public void StopPeriodically()
        {
	        updateCheckerTimer.Dispose();
	        subscriptionUpdaterTimer.Dispose();
        }

        public async void CheckUpdate(object state)
        {
	        try
	        {
		        if (!await firebaseService.AuthorizeUser()) return;

		        var appUpdateResult = await firebaseService.CheckAppVersion();
		        if (appUpdateResult != AppActions.Update && appUpdateResult != AppActions.Continue) return;

		        var dataUpdateResult = await firebaseService.LookForUpdates();
		        if (dataUpdateResult != AppActions.Update) return;

		        Device.BeginInvokeOnMainThread(() => { Settings.IsUpdateAvailable = true; });
                accountRepository.AddUpdateAvailableDate();
	        }
	        catch(Exception e)
	        {
				Debug.WriteLine(e.Message);
	        }
        }
    }
}
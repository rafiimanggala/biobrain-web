using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Extensions;
using BioBrain.Helpers;
using BioBrain.Models;
using BioBrain.Services.Interfaces;
using Common;
using Common.Enums;
using Common.ErrorHandling;
using Common.Interfaces;
using CustomControls.Dialogs;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
using Newtonsoft.Json;
// using Plugin.Connectivity; // Use Microsoft.Maui.Networking.Connectivity
using Plugin.InAppBilling;
// using Unity; // Replaced by MAUI DI
// using Version.Plugin; // TODO: Replace with MAUI equivalent
using Microsoft.Maui.Controls;
// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

// Resolve ambiguity between Common.Enums.AppActions and Microsoft.Maui.ApplicationModel.AppActions
using AppActions = Common.Enums.AppActions;

namespace BioBrain.Services.Implementations
{
	public class FirebaseService : IFirebaseService
    {

	    private const string DemoProductId = "demo";

#if DEBUG
#if Biology
        //private const string ProductId = "android.test.purchased";
#if US
        private const string ProductId = "biobrain.biology.us";
        private const string MonthSubscriptionId = "biology.us.1month";
        private const string YearSubscriptionId = "biology.us.1year";
#elif EU
        private const string ProductId = "biobrain.biology.eu";
        private const string MonthSubscriptionId = "biology.eu.1month";
        private const string YearSubscriptionId = "biology.eu.1year";
#else
        private const string ProductId = "biobrain.biology";
        private const string MonthSubscriptionId = "biology.au.1month";
        private const string YearSubscriptionId = "biology.au.1year";
#endif
#elif Chemistry
        //private const string ProductId = "android.test.purchased";
#if US
        private const string ProductId = "biobrain.chemistry.us";
        private const string MonthSubscriptionId = "chemistry.us.1month";
        private const string YearSubscriptionId = "chemistry.us.1year";
#elif EU
        private const string ProductId = "biobrain.chemistry.eu";
        private const string MonthSubscriptionId = "chemistry.eu.1month";
        private const string YearSubscriptionId = "chemistry.eu.1year";
#else
        private const string ProductId = "biobrain.chemistry";
        private const string MonthSubscriptionId = "chemistry.au.1month";
        private const string YearSubscriptionId = "chemistry.au.1year";
#endif
#elif Physics
        //private const string ProductId = "android.test.purchased";
#if US
        private const string ProductId = "biobrain.physics.us";
        private const string MonthSubscriptionId = "physics.us.1month";
        private const string YearSubscriptionId = "physics.us.1year";
#elif EU
        private const string ProductId = "biobrain.physics.eu";
        private const string MonthSubscriptionId = "physics.eu.1month";
        private const string YearSubscriptionId = "physics.eu.1year";
#else
        private const string ProductId = "biobrain.physics";
        private const string MonthSubscriptionId = "physics.au.1month";
        private const string YearSubscriptionId = "physics.au.1year";
#endif
#endif

#else
#if Biology
#if US
        private const string ProductId = "biobrain.biology.us";
        private const string MonthSubscriptionId = "biology.us.1month";
        private const string YearSubscriptionId = "biology.us.1year";
#elif EU
        private const string ProductId = "biobrain.biology.eu";
        private const string MonthSubscriptionId = "biology.eu.1month";
        private const string YearSubscriptionId = "biology.eu.1year";
#else
        private const string ProductId = "biobrain.biology";
        private const string MonthSubscriptionId = "biology.au.1month";
        private const string YearSubscriptionId = "biology.au.1year";
#endif
#elif Chemistry
#if US
        private const string ProductId = "biobrain.chemistry.us";
        private const string MonthSubscriptionId = "chemistry.us.1month";
        private const string YearSubscriptionId = "chemistry.us.1year";
#elif EU
        private const string ProductId = "biobrain.chemistry.eu";
        private const string MonthSubscriptionId = "chemistry.eu.1month";
        private const string YearSubscriptionId = "chemistry.eu.1year";
#else
        private const string ProductId = "biobrain.chemistry";
        private const string MonthSubscriptionId = "chemistry.au.1month";
        private const string YearSubscriptionId = "chemistry.au.1year";
#endif
#elif Physics
#if US
        private const string ProductId = "biobrain.physics.us";
        private const string MonthSubscriptionId = "physics.us.1month";
        private const string YearSubscriptionId = "physics.us.1year";
#elif EU
        private const string ProductId = "biobrain.physics.eu";
        private const string MonthSubscriptionId = "physics.eu.1month";
        private const string YearSubscriptionId = "physics.eu.1year";
#else
        private const string ProductId = "biobrain.physics";
        private const string MonthSubscriptionId = "physics.au.1month";
        private const string YearSubscriptionId = "physics.au.1year";
#endif
#endif
#endif

        private readonly IAccountRepository accountRepository;
        //private readonly IAccountModel defaultUserModel;
        private readonly IFirebaseRepository firebaseRepository;
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private List<IVersionModel> versions;

        public FirebaseService(IAccountRepository accountRepository, IFirebaseRepository firebaseRepository)
        {
            this.accountRepository = accountRepository;
            //this.defaultUserModel = defaultUserModel;
            this.firebaseRepository = firebaseRepository;
        }

        public bool IsAuthorized => firebaseRepository?.IsAuthorized() ?? false;

        public async Task<bool> CheckIsPurchased(bool checkOnline = true)
        {
	        #if DEBUG
	        logger.Log("CheckIsPurchased skipped in DEBUG");
	        return true; // Treat as purchased in debug to skip StoreKit prompts
	        #endif
	        if (Device.RuntimePlatform == Device.Android)
	        {
		        var connected = await CrossInAppBilling.Current.ConnectAsync();
		        if (!connected) throw new NotConnectedException();
		        if (!IsAuthorized) return false;
		        var user = await firebaseRepository.GetUser();

		        // Delete test purchases for android (anti hack protection?)
		        if (user?.Purchases?.Values.Any(p => p.Id == "transactionId.android.test.purchased") ?? false)
		        {
			        await firebaseRepository.DeletePurchase();
			        UpdateManager.RestoreDbToDemo();
			        return false;
		        }

		        return user?.Purchases?.Any() ?? false;
	        }

	        if (Device.RuntimePlatform == Device.iOS)
	        {
		        var connected = await CrossInAppBilling.Current.ConnectAsync();
                var account = accountRepository.GetAccountModel();
                if(account == null) throw new NoEmailException();

                if (account?.SubscriptionId == PurchaseTypes.FullAppIos.ToString())
                {
                    logger.Log("Check subscription from firebase - FullAppIos");
	                return true;
                }

                if (checkOnline)
		        {
			        try
			        {
				        var user = await firebaseRepository.GetUser();

				        // For app buyers (unlimited subscription)
				        if (user?.Purchases?.Keys.Any(x => x == PurchaseTypes.FullAppIos.ToString()) ?? false)
				        {
					        // Update subscription data
					        account.SubscriptionId = PurchaseTypes.FullAppIos.ToString();
					        accountRepository.Update(account);
					        logger.Log("Check subscription from firebase - FullApp");
                            return true;
				        }
			        }
			        catch (Exception e)
			        {
				        logger.Log("Can't get user from firebase " + e.Message);
			        }
		        }

                // If no subscription locally get from appstore
		        var isLocalData = true;
		        if ((string.IsNullOrEmpty(account?.SubscriptionId) || account.SubscriptionDate == null) && connected)
		        {
			        if (!await UpdateAccountFromAppStore(account))
			        {
				        logger.Log("Check subscription from firebase - No subscription local and AppStore");
                        return false;
			        }
			        isLocalData = false;
		        }

                // Subscription expired - try get from appstore and check again
		        if (!CheckSubscription(account))
		        {
			        if (isLocalData && account.SubscriptionId != DemoProductId && connected)
				        if (await UpdateAccountFromAppStore(account))
				        {
					        var result = CheckSubscription(account);
					        logger.Log($"Check - Local subscription not valid. Check AppStore IsPurchased - {result} ({account.SubscriptionId} - {account.SubscriptionDate})");
					        return result;
				        }

			        logger.Log($"Check - Local subscription not valid. ({account.SubscriptionId} - {account.SubscriptionDate})");
                    return false;
		        }

		        logger.Log($"Check - Subscription local valid ({account.SubscriptionId} - {account.SubscriptionDate})");
                return true;
	        }

	        return false;
        }

        private async Task<bool> UpdateAccountFromAppStore(IAccountModel account)
        {
	        var subscription = await GetSubscriptionFromStore();
	        if (subscription == null || string.IsNullOrEmpty(subscription.ProductId)) return false;

	        // Update subscription data
	        account.SubscriptionDate = subscription.TransactionDateUtc;
	        account.SubscriptionId = subscription.ProductId;
	        accountRepository.Update(account);
	        return true;
        }

        private bool CheckSubscription(IAccountModel account)
        {
	        // Check dates
	        var expirationDate = DateTime.MinValue;
	        switch (account.SubscriptionId)
	        {
		        case DemoProductId:
			        return false;
#if DEBUG
		        case MonthSubscriptionId:
		        case YearSubscriptionId:
			        expirationDate = account.SubscriptionDate.Value.AddMinutes(120);
			        break;
#else
                    case MonthSubscriptionId:
	                    expirationDate = account.SubscriptionDate.Value.AddMonths(1);
                        break;
                    case YearSubscriptionId:
	                    expirationDate = account.SubscriptionDate.Value.AddYears(1);
	                    break;
#endif
	        }

	        if (expirationDate < DateTime.UtcNow) return false;

	        return true;
        }

        public async Task<UserFirebaseModel> GetUser()
        {
            var connected = await CrossInAppBilling.Current.ConnectAsync();
            if (!connected) return null;
            if (!IsAuthorized) return null;
            var user = await firebaseRepository.GetUser();
            return user;
        }

        public async Task<bool> ActivatePromo()
        {
            try
            {
	            // No promo-codes on iOS yet
	            if (Device.RuntimePlatform == Device.iOS) return false;

                await AuthorizeUser();
                if (!firebaseRepository.IsAuthorized()) throw new UnauthorizedAccessException();
                var connected = await CrossInAppBilling.Current.ConnectAsync();
                if (!connected) throw new NotConnectedException();

                var purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase);
                foreach (var purchase in purchases.Where(purchase => purchase.State == PurchaseState.Purchased && purchase.ConsumptionState == ConsumptionState.NoYetConsumed))
                {
                    // v7: ConsumePurchaseAsync takes productId and purchaseToken
                    await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);

                    //Purchased, save this information
                    var purchaseModel = App.Container.Resolve<IFirebasePurchaseModel>();
                    purchaseModel.Date = DateTime.Now.ConvertToUnixTimestamp();
                    purchaseModel.Id = purchase.Id;
                    purchaseModel.Token = purchase.PurchaseToken;
                    purchaseModel.State = purchase.State.ToString();

                    await firebaseRepository.AddPurchase(purchaseModel, PurchaseTypes.Promo);
                    return true;
                }
                return false;
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        public async Task<InAppBillingProduct> GetCost()
        {
	        return await GetCost(ProductId, ItemType.InAppPurchase);
        }

        public async Task<InAppBillingProduct> GetMonthCost()
        {
	        return await GetCost(MonthSubscriptionId, ItemType.Subscription);
        }

        public async Task<InAppBillingProduct> GetYearCost()
        {
	        return await GetCost(YearSubscriptionId, ItemType.Subscription);
        }

        public async Task<InAppBillingProduct> GetCost(string id, ItemType type)
        {
            logger.Log($"Get price for product {id}");
            #if DEBUG
            logger.Log("Failed to get product info.");
            return null;
            #endif
            var connected = await CrossInAppBilling.Current.ConnectAsync();
            if (!connected) return null;

            await CrossInAppBilling.Current.ConnectAsync();
            var productInfo = await CrossInAppBilling.Current.GetProductInfoAsync(type, id);
            var purchase = productInfo.FirstOrDefault();
            await CrossInAppBilling.Current.DisconnectAsync();

            //if (purchase == null) return string.Empty;
            return purchase;
        }

        public async Task<bool> MakePurchase()
        {
            return await Purchase(ProductId, ItemType.InAppPurchase);
        }

        public async Task<bool> PurchaseSubscription(SubscriptionDialogResult subscriptionType)
        {
            string productId;
            switch (subscriptionType)
            {
                case SubscriptionDialogResult.Month:
                    productId = MonthSubscriptionId;
                    break;
                case SubscriptionDialogResult.Year:
                    productId = YearSubscriptionId;
                    break;
                case SubscriptionDialogResult.Cancel:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionType), subscriptionType, null);
            }
            logger.Log($"Purchase - {productId}");

            return await Purchase(productId, ItemType.Subscription);
        }

        private async Task<bool> Purchase(string productId, ItemType itemType)
        {
            try
            {
                #if DEBUG
                logger.Log($"Purchase skipped in DEBUG: {productId}");
                return false;
                #endif
                if (string.IsNullOrEmpty(productId)) return false;

                await AuthorizeUser();
                if (!firebaseRepository.IsAuthorized()) throw new UnauthorizedAccessException();
                var connected = await CrossInAppBilling.Current.ConnectAsync();
                if (!connected) throw new NotConnectedException();

                //try to purchase item
                var purchase =
                    await CrossInAppBilling.Current.PurchaseAsync(productId, itemType, "app-payload");
                if (purchase == null)
                {
	                logger.Log($"Purchase - null result from CrossInAppBilling");
                    throw new PurchaseException();
                }

                if (itemType != ItemType.Subscription)
                {
	                logger.Log($"Purchase - not subscription");
                    // v7: ConsumePurchaseAsync signature changed
                    await CrossInAppBilling.Current.ConsumePurchaseAsync(productId, "app-payload");
                }

                logger.Log($"Purchase - result: {JsonConvert.SerializeObject(purchase)}");

                //Purchased, save this information
                var purchaseModel = App.Container.Resolve<IFirebasePurchaseModel>();
                purchaseModel.ProductId = purchase.ProductId;
                purchaseModel.Date = purchase.TransactionDateUtc.ConvertToUnixTimestamp();
                purchaseModel.Id = purchase.Id;
                purchaseModel.Token = purchase.PurchaseToken;
                purchaseModel.State = purchase.State.ToString();

                if (purchase.State == PurchaseState.Canceled) return false;
                if (purchase.State != PurchaseState.Purchased) throw new PurchaseException();

                Settings.ContentType = AppContentType.Full;
                await firebaseRepository.AddPurchase(purchaseModel, itemType == ItemType.Subscription ? PurchaseTypes.Subscription : PurchaseTypes.PurchaseAll);
                return true;
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
                throw e;
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }


        private IVersionModel GetVersion()
        {
            return versions.OrderByDescending(v => v.DataVersion).FirstOrDefault() ?? new VersionModel();
        }

        public async Task UpdateDemo(IProgress<int> progressCallback)
        {
            var version = GetVersion();

            if (string.IsNullOrEmpty(version.Key)) return;
            if (!UpdateManager.IsUpdateNeed(version.DataVersion)) return;

            var isSendMetrics = await firebaseRepository.GetIsGetMetrics();
            var filesList = await firebaseRepository.GetDemoFilesList(version.Key);
            var updateResult = await UpdateManager.UpdateFiles(filesList.Values.ToList(), progressCallback);
            if (isSendMetrics)
                await SendLog(
                    $"Total: {updateResult.UpdateProcessMetric}; Download: {updateResult.DownloadProcessMetric}",
                    LogTypes.UpdateMetric);

            if (!updateResult.IsSuccess)
            {
                await SendLog(string.Empty, LogTypes.UpdateError);
                return;
            }

            var databaseInfo = await firebaseRepository.GetDemoDatabaseInfo(version.Key);
            if (!await UpdateManager.UpdateDatabase(databaseInfo, AppContentType.Demo))
                await SendLog(string.Empty, LogTypes.UpdateError);

            UpdateManager.StoreVersion(version.DataVersion.ToString(), version.StructureVersion.ToString());
        }

        public async Task<CheckResult> UpdateFull(IProgress<int> progressCallback, bool forceDownloadLatest = false)
        {
            var version = GetVersion();

            if (string.IsNullOrEmpty(version.Key))
            {
                logger.Log("Update full - No version to get.");
	            return CheckResult.Fail(StringResource.NoUpdatesAvailableString);
            }

            if (!UpdateManager.IsUpdateNeed(version.DataVersion) && !forceDownloadLatest)
            {
	            logger.Log("Update full - Update not need");
                return CheckResult.Success();
            }
            
            var isSendMetrics = await firebaseRepository.GetIsGetMetrics();
            var filesList = await firebaseRepository.GetDemoFilesList(version.Key);
            var fullFilesList = await firebaseRepository.GetFullFilesList(version.Key);
            fullFilesList.ForEach(f => filesList.Add(f.Key, f.Value));
            var updateResult = await UpdateManager.UpdateFiles(filesList.Values.ToList(), progressCallback);
            if (isSendMetrics)
                await SendLog(
                    $"Total: {updateResult.UpdateProcessMetric}; Download: {updateResult.DownloadProcessMetric}",
                    LogTypes.UpdateMetric);

            if (!updateResult.IsSuccess)
            {
                await SendLog(string.Empty, LogTypes.UpdateError);
            }

            var databaseInfo = await firebaseRepository.GetFullDatabaseInfo(version.Key);
            if (!await UpdateManager.UpdateDatabase(databaseInfo, AppContentType.Full))
            {
	            await SendLog(string.Empty, LogTypes.UpdateError);
	            return CheckResult.Fail();
            }

            databaseInfo = await firebaseRepository.GetDemoDatabaseInfo(version.Key);
            if (!await UpdateManager.UpdateDatabase(databaseInfo, AppContentType.Demo))
                await SendLog(string.Empty, LogTypes.UpdateError);

            UpdateManager.StoreVersion(version.DataVersion.ToString(), version.StructureVersion.ToString());
            return CheckResult.SuccessWithFails(updateResult.FilesFailed);
        }

        public async Task<CheckResult> CheckFiles(AppContentType contentType)
        {
	        var version = GetVersion();

	        if (string.IsNullOrEmpty(version.Key)) return CheckResult.Fail();

	        var filesList = await firebaseRepository.GetDemoFilesList(version.Key);
	        if (contentType == AppContentType.Full)
	        {
		        var fullFilesList = await firebaseRepository.GetFullFilesList(version.Key);
		        fullFilesList.ForEach(f => filesList.Add(f.Key, f.Value));
	        }

	        var updateResult = UpdateManager.CheckFiles(filesList.Values.ToList());
            
	        return CheckResult.SuccessWithFails(updateResult.FilesFailed);
        }

        public async Task<bool> CheckIsDemo(bool checkOnline = true)
        {
            return !await CheckIsPurchased(checkOnline);
        }

        //private async Task<InAppBillingPurchase> GetSubscription()
        //{
        //    try
        //    {
	       //     var account = accountRepository.GetAccountModel();
	       //     if (account?.SubscriptionDate == null) throw new NullReferenceException();

	       //     return new InAppBillingPurchase
	       //     {
		      //      TransactionDateUtc = account.SubscriptionDate.Value,
		      //      ProductId = account.SubscriptionId
	       //     };
        //    }
        //    catch (Exception e)
        //    {
	       //     logger.Log($"Error while get subscription data from file {e}");
	       //     return await GetSubscriptionFromStore();
        //    }
        //}

        public async Task<InAppBillingPurchase> GetSubscriptionFromStore()
        {
	        #if DEBUG
	        return null;
	        #endif
	        try
	        {
		        if (CrossConnectivity.Current.IsConnected)
		        {
			        if (!firebaseRepository.IsAuthorized()) throw new UnauthorizedAccessException();
			        var connected = await CrossInAppBilling.Current.ConnectAsync();
			        if (!connected) throw new NotConnectedException();

			        var subscriptions = (await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription))?
				        .OrderByDescending(x => x.TransactionDateUtc);

                    logger.Log($"Request purchases Result:\n {JsonConvert.SerializeObject(subscriptions)}");

			        try
			        {
				        await firebaseRepository.AddPurchaseHistory(subscriptions?.Take(10).Select(x =>
					        new FirebasePurchaseModel
					        {
						        ProductId = x.ProductId,
						        Date = x.TransactionDateUtc.ConvertToUnixTimestamp(),
						        DisplayDate = x.TransactionDateUtc.ToString("u"),
						        Id = x.Id,
						        State = x.State.ToString(),
						        Token = x.PurchaseToken
					        }));
			        }
			        catch
			        {
				        logger.Log("Error update purchase history");
				        // Ignored
			        }

			        var purchase = subscriptions?.FirstOrDefault(x => x.State == PurchaseState.Purchased);

			        return purchase ?? new InAppBillingPurchase
				        {ProductId = DemoProductId, TransactionDateUtc = DateTime.MinValue};
		        }
		        else
			        throw new NotConnectedException();
	        }
	        catch (Exception e)
	        {
		        logger.Log($"Error while get subscription from store {e.Message}");
		        return null;
	        }
	        finally
	        {
		        await CrossInAppBilling.Current.DisconnectAsync();
	        }
        }

        public async Task<AppActions> LookForUpdates()
        {
            if (!CrossConnectivity.Current.IsConnected)
                return UpdateManager.IsSuitableStructure() ? AppActions.Continue : AppActions.Lock;

            var structVersion = UpdateManager.AppStructureVersion;
            var dataVersion = UpdateManager.AppDataVersion;
            try
            {
                versions = (await firebaseRepository.GetVersions())
                    .Where(v => v.StructureVersion == structVersion && v.DataVersion >= dataVersion).ToList();
            }
            catch(Exception e)
            {
                logger.Log($"Get Versions: {e}");
                return UpdateManager.IsSuitableStructure() ? AppActions.Continue : AppActions.Lock;
            }

            if(!versions.Any(v => v.DataVersion > dataVersion))
                return UpdateManager.IsSuitableStructure() ? AppActions.Continue : AppActions.Lock;

            logger.Log($"Data version current: {dataVersion}, remote: {versions?.Max(x => x.DataVersion)}");

            return AppActions.Update;
        }

        public async Task<AppActions> CheckAppVersion()
        {
            //Check internet
            if (!CrossConnectivity.Current.IsConnected)
                return UpdateManager.IsSuitableStructure() ? AppActions.Continue : AppActions.Lock;

            var currentAppVersion = AppHelper.AppVersion;
            var minimumAppVersion = await firebaseRepository.GetMinimumAppVersion();

            return string.IsNullOrEmpty(minimumAppVersion) 
                ? AppActions.Continue 
                : StringsHelper.IsVersionSupported(currentAppVersion, minimumAppVersion) 
                    ? AppActions.Continue : AppActions.Lock;
        }

        public async Task<string> GetAppVersionMessage()
        {
            //Check internet
            if (!CrossConnectivity.Current.IsConnected)
                return StringResource.AppUpdateNeedString;

            var message = await firebaseRepository.GetMinimumAppVersionMessage();

            return string.IsNullOrEmpty(message) ? StringResource.AppUpdateNeedString : message;
        }

        public IAccountModel GetAccount()
        {
            var acc = accountRepository.GetAccountModel();
            logger.Log($"Email: {acc?.Email}");
            return acc;
        }

        public bool CheckUserData()
        {
            //Get email from database or from android
            var user = accountRepository.GetRegisterModel();
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password)) return false;

            return true;
        }

        public async Task<bool> AuthorizeUser()
        {
            //Check internet
            if (!CrossConnectivity.Current.IsConnected) throw new NotConnectedException();

            //Get email from database or from android
            var account = GetAccount();

            if (string.IsNullOrEmpty(account?.Email)) return false;

            //Login to firebase

            try
            {
                await Login(account.Email, account.Password);

            }
            catch (FirebaseException e)
            {
                if (e.ErrorType == ErrorType.AuthorizationError)
                {
                    Debug.WriteLine("---=====" + e.Message);
                    logger.Log($"LoginError: {account.Email}");
                }
                else
                {
                    Debug.WriteLine("---=====" + e.Message);
                    logger.Log($"UnhandledLoginError: {e.Message}");
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("---=====" + e.Message);
                logger.Log($"LoginException: {e.Message}");
                return false;
            }

            return true;
        }

        public async Task SendLog(string message, string type)
        {
            var log = logger.GetLog() ?? string.Empty;
            var totalLog = $"{{Message:\"{message}\"; Log: \"{log}\"}}"; 
            await firebaseRepository.SendLog(totalLog, CrossVersion.Current.Version, type);
        }

        public async Task<bool> Login(string email, string password)
        {
            var version = CrossVersion.Current.Version;
            var isLoggedIn = await firebaseRepository.Authorize(email, password, version);
            if (isLoggedIn)
            {
                logger.Log($"Login: {email}");
                Debug.WriteLine($"---=====Login: {email}");
            }

            return isLoggedIn;
        }

        public async Task Register(IRegistrationModel model)
        {
                var isRegistered = await firebaseRepository.Register(model);
                if (isRegistered)
                {
                    logger.Log($"Register: {model.Email}");
                    Debug.WriteLine($"Register: {model.Email}");
                }
        }

        public async Task ResetPassword(string email)
        {
            await firebaseRepository.ResetPassword(email);
        }

        public async Task Update(IRegistrationModel model)
        {
            await firebaseRepository.UpdateUser(model);
        }

        public async Task<string> UploadAvatar(string path)
        {
            var url = await firebaseRepository.UploadAvatar(path);
            return url;
        }

		public async Task SaveIosPurchaseIfNotExists()
		{
			try
			{
				if (Device.RuntimePlatform != Device.iOS) return;
				#if DEBUG
				logger.Log("SaveIosPurchaseIfNotExists skipped in DEBUG");
				return;
				#endif
				if (logger.OldLogFileExist() && !await CheckIsPurchased())
				{
					await firebaseRepository.AddPurchase(
						new FirebasePurchaseModel {Date = DateTime.Now.ConvertToUnixTimestamp()},
						PurchaseTypes.FullAppIos);

                    //Add local
                    // Update subscription data
                    var account = accountRepository.GetAccountModel();
                    account.SubscriptionId = PurchaseTypes.FullAppIos.ToString();
                    accountRepository.Update(account);
                }
			}
			catch (Exception e)
			{
                logger.Log("Error Save Ios Purchase: " + e.Message);
			}
		}

        public async Task DeleteAccount(string reason)
        {
            if (!await AuthorizeUser())
            {
                throw new FirebaseException(ErrorType.AuthorizationError);
            }

            // SendReason
            await firebaseRepository.AddReason(reason);
            Debug.WriteLine("DeleteAcc - Added reason");
            // Delete user data from database
            await firebaseRepository.DeleteUser();
            Debug.WriteLine("DeleteAcc - User Data Deleted");
            // Delete user account
            await firebaseRepository.DeleteAccount();
            Debug.WriteLine("DeleteAcc - User Account deleted");
            //Delete local account
            accountRepository.RemoveLocalAccount();

        }
	}
}
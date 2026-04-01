using System;
using System.Threading.Tasks;
using BioBrain.Services.Interfaces;
using Common;
using Common.Enums;
using Common.ErrorHandling;
using Common.Interfaces;
using CustomControls.Dialogs;
using DAL.Models.Interfaces;
using Plugin.InAppBilling;
// using Unity; // Replaced by MAUI DI
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels
{
    public class BasePurchasableViewModel : BaseWebViewModel, IBasePurchasableViewModel
    {
        private readonly IFirebaseService firebaseService = App.Container.Resolve<IFirebaseService>();
        //public event EventHandler<string> Error;
        public event EventHandler<string> AuthError;
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();


        public async Task<bool> MakePurchase()
        {
            try
            {
                return await firebaseService.MakePurchase();
            }
            catch (UnauthorizedAccessException)
            {
                OnAuthError(StringResource.SelectEmailString);
            }
            catch (NoEmailException)
            {
                OnAuthError(StringResource.SelectEmailString);
            }
            catch (NotConnectedException)
            {
                OnError(StringResource.NotConnectedException);
            }
            catch (PurchaseException)
            {
                OnError(StringResource.PurchaseError);
            }
            catch (Exception e)
            {
                var message = e.Message ?? StringResource.PurchaseError;
                if (message.ToLower().Contains("parameter name: source"))
                    message = StringResource.YourNotSignedGPString;
                OnError(message);
            }
            return false;
        }

        public async Task<bool> MakePurchase(SubscriptionDialogResult result)
        {
            try
            {
                return await firebaseService.PurchaseSubscription(result);
            }
            catch (UnauthorizedAccessException)
            {
                OnAuthError(StringResource.SelectEmailString);
            }
            catch (NoEmailException)
            {
                OnAuthError(StringResource.SelectEmailString);
            }
            catch (NotConnectedException)
            {
                OnError(StringResource.NotConnectedException);
            }
            catch (PurchaseException)
            {
                OnError(StringResource.PurchaseError);
            }
            catch (InAppBillingPurchaseException e)
            {
                logger.Log($"Purchase exception: {e.PurchaseError.ToString()}");

                string message;
                switch (e.PurchaseError)
                {
                    case PurchaseError.UserCancelled:
                        message = StringResource.PurchaseError + $" {StringResource.UserCanceledError}";
                        break;
                    case PurchaseError.PaymentNotAllowed:
                        message = StringResource.PurchaseError + $" {StringResource.PaymentNotAllowedError}";
                        break;
                    case PurchaseError.PaymentInvalid:
                        message = StringResource.PurchaseError + $" {StringResource.PaymentInvalidError}";
                        break;
                    case PurchaseError.InvalidProduct:
                        message = StringResource.PurchaseError + $" {StringResource.InvalidProductError}";
                        break;
                    case PurchaseError.AlreadyOwned:
                        message = StringResource.PurchaseError + $" {StringResource.AlreadyOwnedError}";
                        break;
                    default:
                        message = StringResource.PurchaseError + $" {e.Message}";
                        break;
                }

                OnError(message);
            }
            catch (Exception e)
            {
                var message = e.Message ?? StringResource.PurchaseError;
                if (message.ToLower().Contains("parameter name: source"))
                    message = StringResource.YourNotSignedGPString;
                OnError(message);
            }
            return false;
        }

        public async Task<InAppBillingProduct> GetPrice()
        {
            try
            {
                return await firebaseService.GetCost();
            }
            catch (Exception)
            {
	            logger.Log(StringResource.GetPriceErrorString);
                OnError(StringResource.GetPriceErrorString);
            }
            return null;
        }

        public async Task<InAppBillingProduct> GetMonthPrice()
        {
	        try
	        {
		        return await firebaseService.GetMonthCost();
	        }
	        catch (Exception)
	        {
                logger.Log(StringResource.GetPriceErrorString);
		        //OnError(StringResource.GetPriceErrorString);
	        }
	        return null;
        }

        public async Task<InAppBillingProduct> GetYearPrice()
        {
	        try
	        {
		        return await firebaseService.GetYearCost();
	        }
	        catch (Exception e)
	        {
		        var a = e.Message;
		        logger.Log(StringResource.GetPriceErrorString);
                //OnError(StringResource.GetPriceErrorString);
            }
	        return null;
        }

        public async Task<bool> IsPurchased()
        {
            try
            {
                return await firebaseService.CheckIsPurchased();
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }
            return false;
        }

        public bool IsAuthorized => firebaseService.IsAuthorized;

        public async Task<bool> Authorize()
        {
            return await firebaseService.AuthorizeUser();
        }

        //public async Task GetEmailAndRetry()
        //{
        //    var email = GetAccount();
        //    if (string.IsNullOrEmpty(email)) return;
        //    await MakePurchase();
        //}

        public IAccountModel GetAccount()
        {
            return firebaseService.GetAccount();
        }

        //protected virtual void OnError(string e)
        //{
        //    Error?.Invoke(this, e);
        //}

        protected virtual void OnAuthError(string e)
        {
            AuthError?.Invoke(this, e);
        }

        public async Task SendLogs()
        {
	        await firebaseService.SendLog("Purchase fail", LogTypes.PaymentError);
        }
    }
}
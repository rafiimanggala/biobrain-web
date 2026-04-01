using System;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.ErrorHandling;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class AboutBiobrainViewModel : BasePurchasableViewModel, IAboutBiobrainViewModel
    {
        private readonly IFirebaseService firebaseService;

        public AboutBiobrainViewModel(IFirebaseService firebaseService)
        {
            this.firebaseService = firebaseService;
        }

        public string PromoCode { get; set; }
        public bool IsSendPromocod { get; private set; }
        public bool IsPromoVisible => Device.RuntimePlatform == Device.Android;

        public async void SendPromocode()
        {
            //Send promo
            await Launcher.OpenAsync(new Uri(Settings.PromoGooglePlayUrl + PromoCode));
            IsSendPromocod = true;
            PromoCode = string.Empty;
        }

        //public async Task GetEmailAndRetrySend()
        //{
        //    //Send promo
        //    var email = await firebaseService.GetEmail();
        //    if (string.IsNullOrEmpty(email)) return;
        //    SendPromocode();
        //}

        public bool CheckIsAuthorized()
        {
            if(!firebaseService.IsAuthorized)
                OnAuthError(StringResource.SelectEmailString);
            return firebaseService.IsAuthorized;
        }

        public async Task<bool> ActivatePromo()
        {
            try
            {
                //Activate promo
                return await firebaseService.ActivatePromo();
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
                OnError(e.Message);
            }
            finally
            {
                IsSendPromocod = false;
            }
            return false;
        }
    }
}
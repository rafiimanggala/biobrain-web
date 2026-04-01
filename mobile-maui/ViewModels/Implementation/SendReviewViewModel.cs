using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class SendReviewViewModel : BaseWebViewModel, ISendReviewViewModel
    {
        private string review;
        private readonly IAccountDataStoreManager accountDataStoreManager = DependencyService.Get<IAccountDataStoreManager>();
        private readonly IFirebaseRepository firebaseRepository;
        private readonly IErrorLog loger = DependencyService.Get<IErrorLog>();

        public SendReviewViewModel(IFirebaseRepository firebaseRepository)
        {
            this.firebaseRepository = firebaseRepository;
        }

        public string Review {
            get { return review; }
            set
            {
                review = value;
                OnPropertyChanged(nameof(Review));
                OnPropertyChanged(nameof(IsCanSend));
            }
        }

        public bool IsCanSend => !string.IsNullOrEmpty(Review);

        public void SaveRateResult(RateResult result)
        {
            accountDataStoreManager.SaveUserRateResult(result);
            App.IsRateShown = true;
        }

        public async Task SendReview()
        {
            if (string.IsNullOrEmpty(review)) return;
            try
            {
                StartProcess();
                await firebaseRepository.SendReview(review);
            }
            catch (Exception e)
            {
                loger.Log(e.Message);
                Debug.WriteLine(e.InnerException);
            }
            finally
            {
                SaveRateResult(RateResult.Review);
                EndProcess();
            }
        }
    }
}
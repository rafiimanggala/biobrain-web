using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BioBrain.Annotations;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
// using Version.Plugin; // TODO: Replace with MAUI equivalent
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class FeedbackViewModel : IFeedbackViewModel, INotifyPropertyChanged
    {
        private readonly IErrorLog loger = DependencyService.Get<IErrorLog>();
        private readonly IFirebaseRepository firebaseRepository;
        private readonly IFirebaseService firebaseService;
        private readonly IAccountRepository accountRepository;
        public bool IsBusy { get; private set; }

        public FeedbackViewModel(IFirebaseRepository firebaseRepository, IFirebaseService firebaseService, IAccountRepository accountRepository)
        {
            this.firebaseRepository = firebaseRepository;
            this.firebaseService = firebaseService;
            this.accountRepository = accountRepository;
        }

        public async Task<string> SendLog()
        {
            try
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));

                var log = loger.GetLog();
                if (string.IsNullOrEmpty(log)) return StringResource.EmptyLogsError;
                
                var acc = accountRepository.GetRegisterModel();
                if(acc == null) throw new UnauthorizedAccessException(StringResource.UnhandledLoginError);

                await firebaseService.Login(acc.Email, acc.Password);
                await firebaseRepository.SendLog(log, CrossVersion.Current.Version);

                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                return StringResource.LogsSendedMessage;
            }
            catch (Exception)
            {
                return StringResource.SendLogError;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
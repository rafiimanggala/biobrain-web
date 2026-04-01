using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BioBrain.Annotations;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.ErrorHandling;
using Common.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class DeleteAccountViewModel: IDeleteAccountViewModel, INotifyPropertyChanged
    {
        public event EventHandler<string> Error;
        public event EventHandler FinishDelete;

        private bool isBusy = false;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private DeleteAccountStage _step = DeleteAccountStage.Description;
        public DeleteAccountStage Step
        {
            get => _step;
            set
            {
                _step = value;
                OnPropertyChanged(nameof(Step));
                OnPropertyChanged(nameof(IsDescriptionVisible));
                OnPropertyChanged(nameof(IsReasonVisible));
                OnPropertyChanged(nameof(IsResultVisible));
            }
        }

        public bool IsDescriptionVisible => Step == DeleteAccountStage.Description;
        public bool IsReasonVisible => Step == DeleteAccountStage.Reason;
        public bool IsResultVisible => Step == DeleteAccountStage.Result;

        private string _reason = string.Empty;
        public string Reason
        {
            get => _reason;
            set
            {
                _reason = value;
                OnPropertyChanged(nameof(IsContinueAvailable));
            }
        }

        public bool IsContinueAvailable => Reason != string.Empty;

        private string _header = StringResource.DeleteMyAccount;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string OkButtonText => StringResource.OkString.ToUpper();
        public string ContinueDeletionButtonText => StringResource.ContinueAccountDeletion.ToUpper();
        public string CancelButtonText => StringResource.Cancel.ToUpper();
        public string ContinueButtonText => StringResource.Continue.ToUpper();

        private readonly IFirebaseService firebaseService;
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        public DeleteAccountViewModel(IFirebaseService firebaseService)
        {
            this.firebaseService = firebaseService;
        }

        public void GoToReason()
        {
            Step = DeleteAccountStage.Reason;
            Header = StringResource.Goodbye;
        }

        public async Task Submit()
        {
            try
            {
                IsBusy = true;
                await firebaseService.DeleteAccount(Reason);
                Step = DeleteAccountStage.Result;
            }
            catch (FirebaseException e)
            {
                if (e.ErrorType == ErrorType.AuthorizationError)
                {
                    Debug.WriteLine("---=====" + e.Message);
                    logger.Log($"LoginError");
                    OnError("Authorization issues");
                }
                else
                {
                    Debug.WriteLine("---=====" + e.Message);
                    logger.Log($"UnhandledLoginError: {e.Message}");
                    OnError("Something go wrong. Please try again.");
                }
            }
            catch (Exception e)
            {
                OnError(e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Finish()
        {
            OnFinishDelete();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnError(string e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnFinishDelete()
        {
            FinishDelete?.Invoke(this, EventArgs.Empty);
        }
    }
}
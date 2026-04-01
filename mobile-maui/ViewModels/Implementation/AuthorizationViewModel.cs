using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.Interfaces;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.ErrorHandling;
using Common.Interfaces;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Plugin.Connectivity; // Use Microsoft.Maui.Networking.Connectivity
// using Unity; // Replaced by MAUI DI
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    internal enum AuthMode
    {
        Login,
        Register,
        Reset
    }

    public class AuthorizationViewModel : BaseWebViewModel, IAuthorizationViewModel
    {
        public event EventHandler Finish;
        public event EventHandler ResetPasswordEmailSent;
        
        private readonly IAccountRepository accountRepository;
        private readonly IFirebaseService firebaseService;
        private IRegistrationModel model;
        private AuthMode mode = AuthMode.Login;
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly ICountriesService countriesService = DependencyService.Get<ICountriesService>();
        private const string DefaultPassword = "02D394C6-04E2-4979-9A66-D7F65663A90B";
        private const string Australia = "Australia";

        public AuthorizationViewModel(IAccountRepository accountRepository, IFirebaseService firebaseService)
        {
            this.accountRepository = accountRepository;
            this.firebaseService = firebaseService;
            GetModel();
            EndProcess();
        }

        public bool IsLoginVisible => mode == AuthMode.Login;

        public bool IsRegistrationVisible => mode == AuthMode.Register;

        public bool IsResetVisible => mode == AuthMode.Reset;

        public List<string> Countries => countriesService.GetCountries();
        public List<string> AustralianStates => new List<string>() { "New South Wales", "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia" };

        public bool IsStateVisible => Country == Australia && IsRegistrationVisible;
        public bool IsPasswordVisible => IsRegistrationVisible || IsLoginVisible;

        public string FirstName
        {
            get => model.FirstName;
            set => model.FirstName = value;
        }

        public string LastName
        {
            get => model.Surname;
            set => model.Surname = value;
        }

        public string AvatarPath
        {
            get => string.IsNullOrEmpty(model.AvatarPath) ? string.Empty : model.AvatarPath;
            set
            {
                model.AvatarPath = value;
                OnPropertyChanged(nameof(AvatarPath));
            }
        }

        public string Email
        {
            get => model.Email;
            set => model.Email = value;
        }

        public string Password
        {
            get => model.Password;
            set => model.Password = value;
        }

        public string ConfirmPassword { get; set; }

        public string Country
        {
            get => string.IsNullOrEmpty(model.Country)? null: model.Country;
            set
            {
                model.Country = value;
                OnPropertyChanged(nameof(IsStateVisible));
            }
        }

        public string State
        {
            get => string.IsNullOrEmpty(model.State) ? null : model.State;
            set => model.State = value;
        }

        private void GetModel()
        {
            var existingModel = accountRepository.GetRegisterModel();
            model = existingModel ?? App.Container.Resolve<IRegistrationModel>();
            OnPropertyChanged(nameof(AvatarPath));
        }

        private void SetMode(AuthMode value)
        {
            mode = value;
            OnPropertyChanged(nameof(IsLoginVisible));
            OnPropertyChanged(nameof(IsRegistrationVisible));
            OnPropertyChanged(nameof(IsStateVisible));
            OnPropertyChanged(nameof(IsPasswordVisible));
            OnPropertyChanged(nameof(IsResetVisible));
        }

        public void SetLoginMode()
        {
            SetMode(AuthMode.Login);
        }

        public void SetRegisterMode()
        {
            SetMode(AuthMode.Register);
        }

        public void SetResetMode()
        {
            SetMode(AuthMode.Reset);
        }

        private async Task UploadAvatar(string path)
        {
            try
            {
                AvatarPath = await firebaseService.UploadAvatar(path);
            }
            catch (Exception e)
            {
                OnError(e.Message);
            }
        }

        public async Task Submit()
        {
            if(IsBusy) return;
            StartProcess();
            try
            {
	            if (!CrossConnectivity.Current.IsConnected) throw new NotConnectedException();
	            model.Email = model.Email.Trim();
	            switch (mode)
	            {
		            case AuthMode.Login:
			            await Login();
			            break;
		            case AuthMode.Register:
			            if (!await Register())
				            return;
			            break;
		            case AuthMode.Reset:
			            if (string.IsNullOrEmpty(Email))
			            {
				            OnError(string.Format(StringResource.RequiredFieldError, nameof(Email)));
				            return;
			            }

			            await firebaseService.ResetPassword(Email);
			            OnResetPasswordEmailSent();
			            return;
		            default:
			            throw new ArgumentOutOfRangeException();
	            }

				// ToDo Remove
				try { await firebaseService.SaveIosPurchaseIfNotExists(); }
				catch (Exception ex) { logger.Log($"SaveIosPurchase skipped: {ex.Message}"); }
				//await firebaseService.CheckIsPurchased();
				OnFinish();
            }
            catch (NotConnectedException)
            {
                logger.Log("No internet");
#if DEBUG
                logger.Log("DEBUG: Bypassing login after NotConnectedException — proceeding with demo data");
                SaveDemoAccountLocally();
                OnFinish();
                return;
#else
                OnError(StringResource.NoInternetError);
#endif
            }
            catch (UnauthorizedAccessException e)
            {
                logger.Log("No email or password");
#if DEBUG
                logger.Log($"DEBUG: Bypassing login after UnauthorizedAccessException — {e.Message}");
                SaveDemoAccountLocally();
                OnFinish();
                return;
#else
                OnError(e.Message);
#endif
            }
            catch (FirebaseException e)
            {
#if DEBUG
                logger.Log($"DEBUG: Firebase auth failed ({e?.ErrorType}: {e?.Message}) — bypassing login with demo data");
                SaveDemoAccountLocally();
                OnFinish();
                return;
#else
                if (e?.ErrorType == ErrorType.EmailError)
                {
                    logger.Log($"EmailError: {Email}; {e?.Message}");
                    OnError(StringResource.InvalidEmailError);
                }
                else if(e?.ErrorType == ErrorType.PasswordError)
                {
                    logger.Log($"PasswordError: {Email}; {e?.Message}");
                    OnError(StringResource.InvalidPasswordError);
                }
                else if(e.ErrorType == ErrorType.TooManyAttemptsError)
                {
                    logger.Log($"TooManyAttemptsError: {Email}; {e?.Message}");
                    OnError(StringResource.TooManyAttemptsError);
                }
                else if (e?.ErrorType == ErrorType.RegistrationUsedEmail)
                {
                    logger.Log($"UsedEmailError: {Email}; {e?.Message}");
                    OnError(StringResource.UsedEmailError);
                }
                else if (e?.ErrorType == ErrorType.RegistrationWeakPassword)
                {
                    logger.Log($"WeakPasswordError: {Password.Length}; {e?.Message}");
                    OnError(StringResource.WeakPasswordError);
                }
                else if (e?.ErrorType == ErrorType.GetAccountError)
                {
                    logger.Log($"GetAccountError: {Email}; {e?.Message}");
                    OnError(StringResource.GetAccountError);
                }
                else if (e?.ErrorType == ErrorType.UpdateAccountError)
                {
                    logger.Log($"UpdateAccountError: {Email}; {e?.Message}");
                    OnError(StringResource.UpdateAccountError);
                }
                else
                {
                    logger.Log($"UnhandledLoginError: {e?.Message}");
                    OnError(StringResource.UnhandledLoginError);
                    await firebaseService.SendLog(e?.ToString() + e?.InnerException?.ToString(), LogTypes.LoginError);
                }
#endif
            }
            //catch (UnauthorizedAccessException e)
            //{
            //    logger.Log($"LoginError: {Email}; {e.Message}");
            //    OnError(e.Message);
            //    await firebaseService.SendLog(e.ToString() + e.InnerException?.ToString(), LogTypes.LoginError);
            //}
            catch (Exception e)
            {
                logger.Log($"LoginError: {Email}; {e.Message}");
#if DEBUG
                logger.Log($"DEBUG: Bypassing login after exception — {e.Message}");
                SaveDemoAccountLocally();
                OnFinish();
                return;
#else
                var a = e.Message;
                OnError(StringResource.UnhandledLoginError);
                await firebaseService.SendLog(e.ToString() + e.InnerException?.ToString(), LogTypes.LoginError);
#endif
            }
            finally
            {
                EndProcess();
            }
        }

#if DEBUG
        /// <summary>
        /// Saves a minimal demo account locally so the app can proceed past login
        /// when Firebase auth is unavailable in DEBUG builds.
        /// </summary>
        private void SaveDemoAccountLocally()
        {
            try
            {
                var email = string.IsNullOrEmpty(Email) ? "debug@demo.local" : Email;
                var accModel = new AccountModel
                {
                    Email = email,
                    Password = "debug-demo",
                    FirstName = "Debug",
                    Surname = "User",
                    Country = "Australia",
                    State = string.Empty,
                    SubscriptionId = string.Empty,
                };

                var existing = accountRepository.GetAccountModel();
                if (existing == null || existing.AccountID < 1)
                    accountRepository.Insert(accModel);
                else
                    accountRepository.Update(accModel);

                logger.Log($"DEBUG: Demo account saved locally for {email}");
            }
            catch (Exception ex)
            {
                logger.Log($"DEBUG: Failed to save demo account — {ex.Message}");
            }
        }
#endif

        private async Task Login()
        {
            if(string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) throw new UnauthorizedAccessException(StringResource.InvalidEmailOrPasswordError);
            await firebaseService.Login(Email, Password);

            // GetData and save locally
            var userData = await firebaseService.GetUser();
            if(userData == null) throw new NullReferenceException(nameof(userData));
            var data = userData.ToRegistrationModel();
            data.AccountID = model.AccountID;
            data.Email = Email;
            data.Password = Password;
            SaveOrUpdateDataLocally(data, userData.Purchases);
        }

        private async Task<bool> Register()
        {
            // Validate
            if(!Validate()) return false;
            // State not needed if state not Australia
            if (Country != Australia) State = string.Empty;

            // Send data
            try
            {
                await firebaseService.Register(model);
                if (!string.IsNullOrEmpty(AvatarPath))
                {
                    await UploadAvatar(AvatarPath);
                    await firebaseService.Update(model);
                }
            }
            catch (FirebaseException e)
            {
                if (e.ErrorType == ErrorType.RegistrationUsedEmail)
                {
                    // If email already exists try to login with default password
                    //  If not success throw
                    try
                    {
                        if (!await firebaseService.Login(Email, DefaultPassword))
                            throw;
                    }
                    catch
                    {
                        throw e;
                    }
                    //  if success update password and data
                    if (!string.IsNullOrEmpty(AvatarPath))
                    {
                        await UploadAvatar(AvatarPath);
                    }
                    await firebaseService.Update(model);
                    return true;
                }

                // If other error throw
                throw;
            }

            // Save data locally
            SaveOrUpdateDataLocally(model, null);

            return true;
        }

        private void SaveOrUpdateDataLocally(IRegistrationModel registrationModel, Dictionary<string, FirebasePurchaseModel> purchases)
        {
	        var accModel = new AccountModel
	        {
		        Email = registrationModel.Email,
		        Password = registrationModel.Password,
		        AccountID = registrationModel.AccountID,
		        FirstName = registrationModel.FirstName,
		        Surname = registrationModel.Surname,
		        State = registrationModel.State,
		        Country = registrationModel.Country,
		        AvatarPath = registrationModel.AvatarPath,
		        SubscriptionId = (purchases?.Keys.Any(x => x == PurchaseTypes.FullAppIos.ToString()) ?? false)
			        ? PurchaseTypes.FullAppIos.ToString()
			        : string.Empty,
	        };
            if (registrationModel.AccountID < 1) accountRepository.Insert(accModel);
            else accountRepository.Update(accModel);
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(Email))
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(Email)));
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(Password)));
                return false;
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(FirstName)));
                return false;
            }
            if (string.IsNullOrEmpty(LastName))
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(LastName)));
                return false;
            }
            if (string.IsNullOrEmpty(Country))
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(Country)));
                return false;
            }
            if (string.IsNullOrEmpty(State) && Country == Australia)
            {
                OnError(string.Format(StringResource.RequiredFieldError, nameof(State)));
                return false;
            }

            if (Password != ConfirmPassword)
            {
                OnError(StringResource.ConfirmPasswordError);
                return false;
            }

            return true;
        }

        protected virtual void OnFinish()
        {
            Finish?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnResetPasswordEmailSent()
        {
            ResetPasswordEmailSent?.Invoke(this, EventArgs.Empty);
        }
    }
}
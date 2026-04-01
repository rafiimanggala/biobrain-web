using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Extensions;
using Common.Enums;
using Common.ErrorHandling;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database.Query;
using Microsoft.Maui.Controls;

namespace DAL.Repositorys.Implementations
{
    public class FirebaseRepository : BaseFirebaseRepository, IFirebaseRepository
    {
        private readonly IAccountRepository accountRepository;
        private IFilesPath Paths => DependencyService.Get<IFilesPath>();

        public FirebaseRepository(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<IEnumerable<IVersionModel>> GetVersions()
        {
	        try
	        {
		        //await Task.Delay(1000);
		        var query = Client.Child(VersionsField);
		        //var data = string.IsNullOrEmpty(AuthData?.FirebaseToken)
		        // ? await query.OnceSingleAsync<Dictionary<string, VersionModel>>()
		        // : await query.WithAuth(AuthData.FirebaseToken).OnceSingleAsync<Dictionary<string, VersionModel>>();

		        var data = await query.OnceSingleAsync<Dictionary<string, VersionModel>>();
		        return data.Values;
	        }
	        catch (HttpRequestException e)
	        {
		        Debug.WriteLine(e.Message);
		        throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError
			        ? ErrorType.AuthorizationError
			        : ErrorType.Unhandled);
	        }
	        catch (Exception e2)
	        {
		        Debug.WriteLine(e2.ToString());
		        throw;
	        }
        }

        public async Task<string> GetMinimumAppVersion()
        {
            try
            {
                var data =
                await
	                Client.Child(Device.RuntimePlatform == Device.iOS ? MinimumIosAppVersion : MinimumAppVersion)
                        .OnceSingleAsync<string>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
	            var msg = e.Message;
	            Debug.WriteLine(e.ToString());
            }

            try
            {
                await Task.Delay(100);
                var data =
                    await
	                    Client.Child(Device.RuntimePlatform == Device.iOS ? MinimumIosAppVersion : MinimumAppVersion)
                            .OnceSingleAsync<string>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<string> GetMinimumAppVersionMessage()
        {
            try
            {
                var data =
                    await
                        Client.Child(MinimumAppVersionMessage)
                            .OnceSingleAsync<string>();
                return data;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task SendLog(string log, string version, string code = LogTypes.Default)
        {
            try
            {
                //if (string.IsNullOrEmpty(AuthData?.FirebaseToken)) return;
                //AuthData = await AuthProvider.SignInWithEmailAndPasswordAsync(AuthData.User.Email, Password);
                await Client.Child(Logs).Child(DateTime.UtcNow.ToString("yyyy/MM/dd/HH:mm:ss"))
                    .PutAsync(new LogModel { Log = log, Code = code, Version = version, Platform = Device.RuntimePlatform, User = accountRepository.GetAccountModel()?.Email ?? AuthData?.User?.Email });
            }
            catch(Exception)
            {
                try
                {
                    //if (string.IsNullOrEmpty(AuthData?.FirebaseToken)) return;
                    //AuthData = await AuthProvider.SignInWithEmailAndPasswordAsync(AuthData.User.Email, Password);
                    await Client.Child(Logs).Child(DateTime.UtcNow.ToString("yyyy/MM/dd/HH:mm:ss")+"_1")
                        .PutAsync(new LogModel { Log = log, Code = code, Version = version, Platform = Device.RuntimePlatform, User = accountRepository.GetAccountModel()?.Email ?? AuthData?.User?.Email });
                }
                catch (Exception)
                {
                    //Ignore
                }
            }
        }

        public async Task SendReview(string review)
        {
            var model = new ReviewModel
            {
                Email = AuthData?.User?.Email ?? string.Empty,
                Body = review,
                CreateAt = DateTime.Now.ToUniversalTime().ConvertToUnixTimestamp(),
                UID = AuthData?.User?.LocalId ?? string.Empty
            };

            if (string.IsNullOrEmpty(AuthData?.FirebaseToken))
                await Client.Child(Reviews).Child(Guid.NewGuid().ToString())
                         .PutAsync(model);
            else
                await Client.Child(Reviews).Child(Guid.NewGuid().ToString())
                         .WithAuth(AuthData.FirebaseToken)
                         .PutAsync(model);
        }

        public async Task<UserFirebaseModel> GetUser()
        {
            try
            {
                var data =
                    await
                        Client.Child(UsersField)
                            .Child(AuthData.User.LocalId)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<UserFirebaseModel>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.GetAccountError : ErrorType.Unhandled);
            }
        }

        public bool IsAuthorized()
        {
            return !string.IsNullOrEmpty(AuthData?.FirebaseToken);
        }

        public async Task<bool> AddPurchase(IFirebasePurchaseModel purchase, PurchaseTypes type)
        {
            try
            {
                await
                    Client.Child(UsersField).Child(AuthData.User.LocalId).Child(PurchasesField).Child(type.ToString())
                        .WithAuth(AuthData.FirebaseToken)
                        .PutAsync(purchase);
                return !string.IsNullOrEmpty(AuthData.FirebaseToken);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
            catch (Exception e2)
            {
	            var a = e2;
	            throw e2;
            }
        }

        public async Task AddPurchaseHistory(IEnumerable<IFirebasePurchaseModel> purchases)
        {
	        try
	        {
		        await
			        Client.Child(UsersField).Child(AuthData.User.LocalId).Child(PurchaseHistoryField)
				        .WithAuth(AuthData.FirebaseToken)
				        .PutAsync(purchases);
	        }
	        catch (HttpRequestException e)
	        {
		        Debug.WriteLine(e.Message);
		        throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
	        }
	        catch (Exception e2)
	        {
		        var a = e2;
		        throw e2;
	        }
        }

        public async Task<bool> DeletePurchase()
        {
            try
            {
                await
                    Client.Child(UsersField).Child(AuthData.User.LocalId).Child(PurchasesField)
                        .WithAuth(AuthData.FirebaseToken)
                        .DeleteAsync();
                return !string.IsNullOrEmpty(AuthData.FirebaseToken);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<Dictionary<string, IFirebaseFileModel>> GetDemoFilesList(string key)
        {
            try
            {
                var data =
                    await
                        Client.Child(DataInfoField).Child(key)
                            .Child(DemoFilesListField)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<Dictionary<string, FirebaseFileModel>>();
                return data.ToDictionary(k=>k.Key, v=>(IFirebaseFileModel)v.Value);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<bool> GetIsGetMetrics()
        {
            try
            {
                var data =
                    await
                        Client.Child(IsGetMetrics).WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<bool>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<IFirebaseFileModel> GetDemoDatabaseInfo(string key)
        {
            try
            {
                var data =
                    await
                        Client.Child(DataInfoField).Child(key)
                            .Child(DemoDatabaseInfoField)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<FirebaseFileModel>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<Dictionary<string, IFirebaseFileModel>> GetFullFilesList(string key)
        {
            try
            {
                var data =
                    await
                        Client.Child(DataInfoField).Child(key)
                            .Child(FullFilesListField)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<Dictionary<string, FirebaseFileModel>>();
                return data?.ToDictionary(k => k.Key, v => (IFirebaseFileModel)v.Value) ?? new Dictionary<string, IFirebaseFileModel>();
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<IFirebaseFileModel> GetFullDatabaseInfo(string key)
        {
            try
            {
                var data =
                    await
                        Client.Child(DataInfoField).Child(key)
                            .Child(FullDatabaseInfoField)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<FirebaseFileModel>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<int> GetDataVersion(string key)
        {
            try
            {
                var data =
                    await
                        Client.Child(DataInfoField).Child(key)
                            .Child(DataVersionField)
                            .WithAuth(AuthData.FirebaseToken)
                            .OnceSingleAsync<int>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<int> GetStructureVersion(string key)
        {
            try
            {
                var query = Client.Child(DataInfoField).Child(key).Child(StructureVersionField);

                var data = string.IsNullOrEmpty(AuthData?.FirebaseToken) 
                    ? await query.OnceSingleAsync<int>()
                    : await query.WithAuth(AuthData.FirebaseToken).OnceSingleAsync<int>();
                return data;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
        }

        public async Task<bool> Authorize(string email, string password, string appVersion)
        {
            try
            {
                //if (Device.RuntimePlatform == Device.iOS) email = Email;
                AuthData = await AuthProvider.SignInWithEmailAndPasswordAsync(email, password);
                await
                    Client.Child(UsersField)
                        .Child(AuthData.User.LocalId)
                        .Child(AppVersionField)
                        .WithAuth(AuthData.FirebaseToken)
                        .PutAsync(appVersion);

                return !string.IsNullOrEmpty(AuthData.FirebaseToken);
            }

            catch (FirebaseIncorrectPasswordException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.PasswordError);
            }
            catch (FirebaseInvalidEmailException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.EmailError);
            }
            catch (FirebaseTooManyAttemptsException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.TooManyAttemptsError);
            }
            catch (Exception e)
            {
                var a = e;
                throw;
            }
        }

        public async Task<bool> Register(IRegistrationModel model)
        {
            try
            {
                AuthData = await AuthProvider.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);
                await
                    Client.Child(UsersField).Child(AuthData.User.LocalId)
                        .WithAuth(AuthData.FirebaseToken)
                        .PutAsync(new UserFirebaseModel
                        {
                            Email = model.Email,
                            Password = model.Password,
                            FirstName = model.FirstName,
                            Surname = model.Surname,
                            Country = model.Country,
                            State = model.State,

                        });
                return !string.IsNullOrEmpty(AuthData.FirebaseToken);
            }
            catch (FirebaseUsedEmailException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.RegistrationUsedEmail);
            }
            catch (FirebaseWeakPasswordException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.RegistrationWeakPassword);
            }
            catch (FirebaseRegisterException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(ErrorType.Unhandled);
            }
        }

        public async Task UpdateUser(IRegistrationModel model)
        {
            try
            {
                // Update password
                await AuthData.UpdatePasswordAsync(model.Password);

                // Update data
                await
                    Client.Child(UsersField).Child(AuthData.User.LocalId)
                        .WithAuth(AuthData.FirebaseToken)
                        .PutAsync(new UserFirebaseModel
                        {
                            Email = model.Email,
                            Password = model.Password,
                            FirstName = model.FirstName,
                            Surname = model.Surname,
                            Country = model.Country,
                            State = model.State,
                            AvatarPath = model.AvatarPath
                        });
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.UpdateAccountError : ErrorType.Unhandled);
            }
        }

        public async Task DeleteUser()
        {
            try
            {
                await
                    Client.Child(UsersField).Child(AuthData.User.LocalId)
                        .WithAuth(AuthData.FirebaseToken)
                        .DeleteAsync();
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.UpdateAccountError : ErrorType.Unhandled);
            }
        }

        public async Task DeleteAccount()
        {
            try
            {
                await AuthData.DeleteAccountAsync();
                AuthData = null;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.UpdateAccountError : ErrorType.Unhandled);
            }
        }

        public async Task<string> UploadAvatar(string path)
        {
            try
            {
                var imagePath = Path.Combine(Paths.AvatarPath, path);
                var url = await Storage.Child(AvatarFolder).Child(Guid.NewGuid().ToString()+Path.GetExtension(path)).PutAsync(File.Open(imagePath, FileMode.Open));
                return url;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return string.Empty;
            }
        }

        public async Task ResetPassword(string email)
        {
            await AuthProvider.SendPasswordResetEmailAsync(email);
        }

        public async Task AddReason(string reason)
        {
            try
            {
                if (string.IsNullOrEmpty(AuthData?.FirebaseToken))
                    await Client.Child(ReasonsField).Child(Guid.NewGuid().ToString())
                        .PutAsync(new {Reason=reason, Date = DateTime.UtcNow.ToString("R")});
                else
                    await Client.Child(ReasonsField).Child(Guid.NewGuid().ToString())
                        .WithAuth(AuthData.FirebaseToken)
                        .PutAsync(new { Reason = reason, Date = DateTime.UtcNow.ToString("R") });
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.ToString());
                throw new FirebaseException(e.Message == FirebasePluginErrors.AuthError ? ErrorType.AuthorizationError : ErrorType.Unhandled);
            }
            catch (Exception e2)
            {
                var a = e2;
                throw e2;
            }
        }
    }
}
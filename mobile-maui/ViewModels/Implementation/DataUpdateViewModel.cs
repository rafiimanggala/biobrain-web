using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using AppActions = Common.Enums.AppActions;
using Common.ErrorHandling;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
	public class DataUpdateViewModel : BasePurchasableViewModel, IDataUpdateViewModel
	{
		public string ProcessLabel
		{
			get => processLabel;
			set
			{
				processLabel = value;
				OnPropertyChanged(nameof(ProcessLabel));
				OnPropertyChanged(nameof(ProcessLabelVisible));
			}
		}

		public string UpdateStatusLabel
		{
			get => updateStatus;
			set
			{
				updateStatus = value;
                OnPropertyChanged(nameof(UpdateStatusLabel));
			}
		}

        public bool DownloadButtonVisible => !IsBusy && !isDone || isError;
        public bool ProcessLabelVisible => !string.IsNullOrEmpty(processLabel);

		public event EventHandler<string> ApplicationUpdateNeed;
		public event EventHandler NoEmailError;
		public event EventHandler NavigateHome;

        private string processLabel = string.Empty;
        private string updateStatus = string.Empty;
        //private bool isDownloading = false;
        private readonly bool isForceDownload = false;
		private readonly IFirebaseService firebaseService;
		private readonly IAccountRepository accountRepository;
        private readonly IProjectDataWorker projectDataWorker = DependencyService.Get<IProjectDataWorker>();
		private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
		private bool isDone = false;
		private bool isError = false;

		public DataUpdateViewModel(
            bool isForceDownload,
	        IFirebaseService firebaseService,
            IAccountRepository accountRepository

            )
        {
	        this.firebaseService = firebaseService;
	        this.isForceDownload = isForceDownload;
	        this.accountRepository = accountRepository;
        }

        public async Task CheckForUpdatesInternal()
		{
			this.StartProcess();
			isError = false;
			isDone = false;
			logger.Log("Check for updates");
            UpdateStatusLabel = StringResource.CheckingForUpdatesString;
            OnPropertiesChanged();
            try
            {
				//Check user data and register if needed
                if (!firebaseService.CheckUserData())
                {
                    OnNoEmailError();
                    return;
                }

                try
                {
                    ProcessLabel = StringResource.DataSyncString;
                    logger.Log("Authorize");

                    // Authorize user
                    if (!await firebaseService.AuthorizeUser())
                    {
                        logger.Log("Update - Auth error");
                        OnNoEmailError();
                        return;
                    }

                    ProcessLabel = StringResource.VersionCheckString;

                    logger.Log("Look for updates");
                    var isUpdateAvailable = false;
                    switch (await firebaseService.LookForUpdates())
                    {
                        case AppActions.Lock:
                            OnApplicationUpdateNeed(await firebaseService.GetAppVersionMessage());
                            isUpdateAvailable = false;
                            return;
                        case AppActions.Update:
                            logger.Log("LookForUpdates - Update");
                            isUpdateAvailable = true;
                            break;
                        case AppActions.Continue:
                            logger.Log($"LookForUpdates - Continue {Settings.IsDemo}");
                            if (!projectDataWorker.FullDataBaseExist() && !Settings.IsDemo)
	                            isUpdateAvailable = true;
                            else
                                isUpdateAvailable = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    logger.Log("Check files");
                    var result = await firebaseService.CheckFiles(Settings.ContentType);

                    if (isUpdateAvailable || result.FailsCount > 0)
                    {
	                    UpdateStatusLabel = StringResource.NewVersionAvailableString;
                        ProcessLabel = StringResource.ClickDownloadString;
                    }
                    else
                    {
	                    UpdateStatusLabel = StringResource.NoUpdatesAvailableString;
	                    ProcessLabel = string.Empty;
                    }

                    // If need download full data after billing for content
                    if (isForceDownload) await DownloadAndUpdate();

                }
                catch (NoEmailException)
                {
                    OnNoEmailError();
                }
                catch (NotConnectedException e)
                {
	                UpdateStatusLabel = StringResource.NoInternetConnection;
                    ProcessLabel = string.Empty;
                    isError = true;
                    logger.Log("No connection to the internet");
                    Debug.WriteLine(e);
                    //OnError("No connection to the internet");
                }
                catch (Exception e)
                {
	                UpdateStatusLabel = StringResource.NoInternetConnection;
	                ProcessLabel = string.Empty;
	                isError = true;
                    var mes = e.Message;
                    logger.Log(e.ToString());
                    ProcessLabel = string.Empty;
                    Debug.WriteLine(e);
                }
                finally
                {
                    EndProcess();
                    App.IsPurchaseMade = false;
                    //ProcessLabel = string.Empty;
                    OnPropertiesChanged();
                }
            }
			catch (Exception e)
			{
				logger.Log(e.ToString());
				this.OnError(StringResource.CheckForUpdatesError);
			}
		}

        public async Task DownloadAndUpdate()
        {
	        this.StartProcess();
	        UpdateStatusLabel = StringResource.DownloadingString;
            OnPropertiesChanged();
            try
            {
	            logger.Log("Update");
                ProcessLabel = StringResource.DataSyncString;
		        var progress = new Progress<int>();
		        progress.ProgressChanged += ProgressCallback;
		        //Get real app version
		        if (Settings.IsDemo)
		        {
			        logger.Log("Update - Demo");
				    await firebaseService.UpdateDemo(progress);
				    UpdateStatusLabel = StringResource.DoneString;
                    ProcessLabel = $"100% complete";
                    isDone = true;
                    Settings.IsUpdateAvailable = false;
                    accountRepository.DeleteUpdateAvailableDate();
                    OnNavigateHome();
                    return;
		        }

		        logger.Log("Update - Full");
		        var result = await firebaseService.UpdateFull(progress, true);

		        if (result.IsSuccess)
		        {
			        UpdateStatusLabel = StringResource.DoneString;
			        isDone = true;
			        Settings.IsUpdateAvailable = false;
			        ProcessLabel = result.FailsCount < 1
				        ? StringResource.FullCompleteString
				        : $"{result.FailsCount} {StringResource.FilesFailedString}";
                    accountRepository.DeleteUpdateAvailableDate();
			        OnNavigateHome();
		        }
		        else
		        {
			        UpdateStatusLabel = string.IsNullOrEmpty(result.Message) ? StringResource.UpdateError : result.Message;
			        ProcessLabel = string.Empty;
			        isError = true;
		        }
            }
            catch (UpdateFilesException e)
            {
	            logger.Log($"{e.FileName} - {e.Message}");
	            UpdateStatusLabel = StringResource.UpdateError;
	            ProcessLabel = string.Empty;
            }
            catch (NotConnectedException e)
            {
	            UpdateStatusLabel = StringResource.NoInternetConnection;
	            ProcessLabel = string.Empty;
	            logger.Log("No connection to the internet");
	            Debug.WriteLine(e);
	            isError = true;
                //OnError("No connection to the internet");
            }
            catch (Exception e)
            {
	            UpdateStatusLabel = StringResource.UpdateError;
	            ProcessLabel = string.Empty;
                logger.Log(e.ToString());
                this.OnError(StringResource.UpdateError);
            }
	        finally
	        {
		        EndProcess();
		        App.IsPurchaseMade = false;
                OnPropertiesChanged();
	        }
        }

        private void ProgressCallback(object sender, int i)
        {
	        ProcessLabel = $"{i}% complete";
	        Debug.WriteLine("I : " + i + "    PL : " + ProcessLabel);
        }

        private void OnPropertiesChanged()
        {
	        OnPropertyChanged(nameof(DownloadButtonVisible));
	        OnPropertyChanged(nameof(UpdateStatusLabel));
	        OnPropertyChanged(nameof(ProcessLabelVisible));
        }

        protected virtual void OnNoEmailError()
        {
	        NoEmailError?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnApplicationUpdateNeed(string e)
        {
	        ApplicationUpdateNeed?.Invoke(this, e);
        }

        protected virtual void OnNavigateHome()
        {
	        NavigateHome?.Invoke(this, EventArgs.Empty);
        }
	}
}
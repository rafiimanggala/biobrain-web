using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Interfaces;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using AppActions = Common.Enums.AppActions;
using Common.ErrorHandling;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class AppVersionEventArgs
    {
        public AppVersionEventArgs(bool isDemo)
        {
            IsDemo = isDemo;
        }

        public bool IsDemo { get; }
    }

    public class AreasOfStudyViewModel : BasePurchasableViewModel, IAreasOfStudyViewModel
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();

        public event EventHandler GetFullData;
        public event EventHandler<string> UpdateNeed;
        public event EventHandler NoEmailError;

        public ObservableCollection<IAreaViewModel> Areas { get; set; } = new ObservableCollection<IAreaViewModel>();

        public ObservableCollection<IAreaViewModel> ComingSoonAreas { get; set; } = new ObservableCollection<IAreaViewModel>();

        public int AreasListHeight => Areas.Count*50 + 5;
        public bool IsTableVisible => Settings.IsPeriodicTableVisible;

        public bool IsComingSoonVisible => ComingSoonAreas.Any();

        public string ProgressLabel { get; private set; }
        public bool ProgressLabelIsVisible => !string.IsNullOrEmpty(ProgressLabel);

#if Biology
        public ImageSource SplashImageSource { get; set; } = Device.RuntimePlatform == Device.Android ? "splash.png" : "unifiedsplash.png";
#elif Chemistry
        public ImageSource SplashImageSource { get; set; } = Device.RuntimePlatform == Device.Android ? "splash_chemistry.png" : "unifiedsplash.png";
#elif Physics
        public ImageSource SplashImageSource { get; set; } = Device.RuntimePlatform == Device.Android ? "splash_physics.png" : "unifiedsplash.png";
#endif

        public string ProcessLabel
        {
            get => processLabel;
            private set
            {
                processLabel = value;
                OnPropertyChanged(nameof(ProcessLabel));
            }
        } 

        private IAreasRepository areasRepository;
        private ITopicsRepository topicsRepository;
        private ILevelTypesRepository levelTypesRepository;
        private IMaterialsRepository materialsRepository;

        private readonly IFirebaseService firebaseService;

        private readonly IAccountRepository accountRepository;

        private bool IsLoaded { get; set; }

        private string processLabel = string.Empty;

        public AreasOfStudyViewModel(
            IAreasRepository areasRepository, 
            ITopicsRepository topicsRepository, 
            IFirebaseService firebaseService, 
            IMaterialsRepository materialsRepository,
            ILevelTypesRepository levelTypesRepository,
            IAccountRepository accountRepository)
        {
            this.areasRepository = areasRepository;
            this.topicsRepository = topicsRepository;
            this.firebaseService = firebaseService;
            this.levelTypesRepository = levelTypesRepository;
            this.materialsRepository = materialsRepository;
            this.accountRepository = accountRepository;
        }

        public void GetAreas()
        {
	        areasRepository = App.Container.Resolve<IAreasRepository>();
	        //doneAreasRepository = App.Container.Resolve<IDoneAreasRepository>();
	        topicsRepository = App.Container.Resolve<ITopicsRepository>();

            var models = areasRepository.GetAll().OrderBy(x => x.AreaName);
            Areas.Clear();
            ComingSoonAreas.Clear();
            foreach (var areaModel in models)
            {
                var area = App.Container.Resolve<IAreaViewModel>(new ParameterOverride("model", areaModel));
                //area.IsDone = doneAreasRepository.IsAreaDone(areaModel.AreaID);
                if (!area.IsDone) area.CompletionString = GetCompletionString(areaModel.AreaID);
                if(area.IsComingSoon) ComingSoonAreas.Add(area);
                else Areas.Add(area);
            }
            OnPropertyChanged(nameof(Areas));
            OnPropertyChanged(nameof(ComingSoonAreas));
            OnPropertyChanged(nameof(IsComingSoonVisible));
            OnPropertyChanged(nameof(AreasListHeight));
        }

        public bool CouldNavigateToArea(int areaId)
        {
	        return topicsRepository.CountForArea(areaId) > 0;
        }

        //public async Task GetEmailAndRetry()
        //{
        //    var email = await firebaseService.GetEmail();
        //    if (string.IsNullOrEmpty(email)) return;
        //    IsLoaded = false;
        //    await SyncData();
        //}

        public async Task<bool> SyncData()
        {
            var isDemo = true;
            if (IsLoaded && !App.IsPurchaseMade) return true;
            StartProcess();

            //Check user data and register if needed
            if (!firebaseService.CheckUserData())
            {
#if DEBUG
                logger.Log("DEBUG: No user data found — continuing with demo data");
#else
                EndProcess();
                OnNoEmailError();
                return false;
#endif
            }

            try
            {
                ProcessLabel = StringResource.LoadingLabelString;
                logger.Log("Login");

                var authSuccess = await firebaseService.AuthorizeUser();
#if DEBUG
                if (!authSuccess)
                {
                    logger.Log("DEBUG: AuthorizeUser failed — continuing with demo data instead of redirecting to login");
                }
#else
                if (!authSuccess)
                {
	                OnNoEmailError();
	                return false;
                }
#endif

                if (authSuccess)
                {
                    try { await firebaseService.SaveIosPurchaseIfNotExists(); }
                    catch (Exception ex) { logger.Log($"SaveIosPurchase skipped: {ex.Message}"); }

                    logger.Log("Check app version");
                    switch (await firebaseService.CheckAppVersion())
                    {
                        case AppActions.Lock:
                            logger.Log("AppVersion - Lock");
                            LockApp();
                            OnUpdateNeed(await firebaseService.GetAppVersionMessage());
                            return false;
                        case AppActions.Update:
                        case AppActions.Continue:
                            logger.Log("AppVersion - Update");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (NoEmailException)
            {
#if DEBUG
                logger.Log("DEBUG: NoEmailException in SyncData — continuing with demo data");
#else
                OnNoEmailError();
#endif
            }
            catch (NotConnectedException e)
            {
                ProcessLabel = string.Empty;
                logger.Log("No connection to the internet");
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                var mes = e.Message;
                logger.Log(e.ToString());
                ProcessLabel = string.Empty;
                Debug.WriteLine(e);
            }

            try
            {
	            // Update purchase status
	            isDemo = await firebaseService.CheckIsDemo();
                var localContentType = UpdateManager.GetLocalContentType();
                logger.Log($"Check content type: Local - {localContentType} Remote:{(isDemo ? "demo" : "full")}");
                if (localContentType == AppContentType.Demo && !isDemo)
	            {
		            OnGetFullData();
		            return true;
	            }

	            var account = accountRepository.GetAccountModel();
	            if (!string.IsNullOrEmpty(account?.UpdateAvailableDate))
	            {
		            if (DateTime.TryParse(account.UpdateAvailableDate, out var updateAvailableDate))
		            {
			            if (DateTime.UtcNow.Year + DateTime.UtcNow.Month > updateAvailableDate.Year + updateAvailableDate.Month)
			            {
				            OnGetFullData();
				            return true;
                        }
		            }
	            }
            }
            catch (Exception e)
            {
	            var mes = e.Message;
	            logger.Log(e.ToString());
	            ProcessLabel = string.Empty;
	            Debug.WriteLine(e);
            }
            finally
            {
	            // SetDatabaseConfig
                Settings.ContentType = isDemo ? AppContentType.Demo : AppContentType.Full;

                IsLoaded = true;
                EndProcess();
                App.IsPurchaseMade = false;
                ((App)Application.Current).StartUpdateChecker();
                ProcessLabel = string.Empty;
                OnPropertyChanged(nameof(ProgressLabel));
                OnPropertyChanged(nameof(ProgressLabelIsVisible));
                OnPropertyChanged(nameof(IsComingSoonVisible));
            }

            //SyncClassKit();
            return true;
        }

        private void SyncClassKit()
        {
	        try
	        {
		        if (Device.RuntimePlatform != Device.iOS) return;
		        if (Settings.IsDemo) return;

		        areasRepository = App.Container.Resolve<IAreasRepository>();
		        topicsRepository = App.Container.Resolve<ITopicsRepository>();
		        materialsRepository = App.Container.Resolve<IMaterialsRepository>();
		        levelTypesRepository = App.Container.Resolve<ILevelTypesRepository>();

                var areas = areasRepository.GetAll().Where(x => !x.IsComingSoon).ToList();
		        var topics = topicsRepository.GetAll();
		        var materials = materialsRepository.GetAll();
		        var levels = levelTypesRepository.GetAll();
		        var classKitManager = DependencyService.Get<IClassKit>();
		        classKitManager.SetupClassKitContexts(areas, topics, materials, levels);
	        }
	        catch(Exception e)
	        {
                logger.Log("Sync Class Kit issue: " + e.Message);
	        }
        }

        //private void ProgressCallback(object sender, int i)
        //{
        //    ProcessLabel = $"Downloading content {i}%";
        //    Debug.WriteLine("I : " + i + "    PL : "+ ProcessLabel);
        //    OnPropertyChanged(nameof(ProgressLabel));
        //    OnPropertyChanged(nameof(ProgressLabelIsVisible));
        //}

        private string GetCompletionString(int areaID)
        {
            return $"{topicsRepository.CountDoneForArea(areaID)}/{topicsRepository.CountForArea(areaID)}";
        }

        //protected virtual void OnDataVersionToLow(bool isDemo)
        //{
        //    DataVersionToLow?.Invoke(this, new AppVersionEventArgs(isDemo));
        //}

        protected virtual void OnNoEmailError()
        {
            NoEmailError?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdateNeed(string message)
        {
            UpdateNeed?.Invoke(this, message);
        }

        protected virtual void OnGetFullData()
        {
	        GetFullData?.Invoke(this, EventArgs.Empty);
        }
    }
}

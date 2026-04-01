using BioBrain.AppResources;
using BioBrain.Factories.Implementation;
using BioBrain.Factories.Interfaces;
using BioBrain.Interfaces;
using BioBrain.Services.Implementations;
using BioBrain.Services.Interfaces;
using BioBrain.Stubs;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using BioBrain.Views;
using CommunityToolkit.Maui;
using Common.Interfaces;
using CustomControls.Interfaces;
using CustomControls.LayoutControls.ChemicalElements;
using DAL;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Implementations;
using DAL.Repositorys.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace BioBrain;

public static class MauiProgram
{
    /// <summary>
    /// Service descriptors snapshot — used by Unity shim to look up interface→implementation mappings.
    /// </summary>
    internal static IServiceCollection ServiceDescriptors { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // --- ViewModels ---
        builder.Services.AddTransient<IAreasOfStudyViewModel, AreasOfStudyViewModel>();
        builder.Services.AddTransient<IAreaViewModel, AreaViewModel>();
        builder.Services.AddTransient<ITopicsViewModel, TopicsViewModel>();
        builder.Services.AddTransient<ITopicViewModel, TopicViewModel>();
        builder.Services.AddTransient<ILevelsViewModel, LevelsViewModel>();
        builder.Services.AddTransient<ILevelViewModel, LevelViewModel>();
        builder.Services.AddTransient<IMaterialsViewModel, MaterialViewModel>();
        builder.Services.AddTransient<IQuestionViewModel, QuestionViewModel>();
        builder.Services.AddTransient<IQuestionsViewModel, QuestionsViewModel>();
        builder.Services.AddTransient<IBaseAnswerViewModel, BaseAnswerViewModel>();
        builder.Services.AddTransient<IResultViewModel, ResultViewModel>();
        builder.Services.AddTransient<IGlossaryViewModel, GlossaryViewModel>();
        builder.Services.AddTransient<ILetterViewModel, LetterViewModel>();
        builder.Services.AddTransient<IWordViewModel, WordViewModel>();
        builder.Services.AddTransient<ITextViewModel, TextViewModel>();
        builder.Services.AddTransient<IQuestionReviewViewModel, QuestionReviewViewModel>();
        builder.Services.AddTransient<IAboutViewModel, AboutViewModel>();
        builder.Services.AddTransient<IAccountViewModel, AccountViewModel>();
        builder.Services.AddTransient<IPurchaseInfoViewModel, PurchaseInfoViewModel>();
        builder.Services.AddTransient<IFeedbackViewModel, FeedbackViewModel>();
        builder.Services.AddTransient<IAboutBiobrainViewModel, AboutBiobrainViewModel>();
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IElementViewModel, ElementViewModel>(builder.Services);
        builder.Services.AddTransient<IElemetsTablePageViewModel, ElementsTablePageViewModel>();
        builder.Services.AddTransient<ISendReviewViewModel, SendReviewViewModel>();
        builder.Services.AddTransient<ISearchViewModel, SearchViewModel>();
        builder.Services.AddTransient<ISearchResultViewModel, SearchResultViewModel>();
        builder.Services.AddTransient<IStatsViewModel, StatsViewModel>();
        builder.Services.AddTransient<IStatListViewModel, StatListViewModel>();
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IResultElement, QuestionResultViewModel>(builder.Services);
        builder.Services.AddTransient<IAuthorizationViewModel, AuthorizationViewModel>();
        builder.Services.AddTransient<IDataUpdateViewModel, DataUpdateViewModel>();
        builder.Services.AddTransient<IDeleteAccountViewModel, DeleteAccountViewModel>();

        // --- Repositories ---
        builder.Services.AddTransient<IAreasRepository, AreasRepository>();
        builder.Services.AddTransient<ITopicsRepository, TopicsRepository>();
        builder.Services.AddTransient<IDoneAreasRepository, DoneAreasRepository>();
        builder.Services.AddTransient<IDoneTopicsRepository, DoneTopicsRepository>();
        builder.Services.AddTransient<ILevelTypesRepository, LevelTypesRepository>();
        builder.Services.AddTransient<IMaterialsRepository, MaterialsRepository>();
        builder.Services.AddTransient<IDoneMaterialsRepository, DoneMaterialsRepository>();
        builder.Services.AddTransient<IQuestionsRepository, QuestionsRepository>();
        builder.Services.AddTransient<IAnswersRepository, AnswersRepository>();
        builder.Services.AddTransient<IDoneQuestionsRepository, DoneQuestionRepository>();
        builder.Services.AddTransient<IGlossaryRepository, GlossaryRepository>();
        builder.Services.AddTransient<IAccountRepository, AccountRepository>();
        builder.Services.AddTransient<IFirebaseRepository, FirebaseRepository>();
        builder.Services.AddTransient<IDatabaseRepository, DatabaseRepository>();

        // --- Models ---
        builder.Services.AddTransient<IDoneAreaModel, DoneAreaModel>();
        builder.Services.AddTransient<IDoneTopicModel, DoneTopicModel>();
        builder.Services.AddTransient<IAccountModel, AccountModel>();
        builder.Services.AddTransient<IFirebasePurchaseModel, FirebasePurchaseModel>();
        builder.Services.AddTransient<IRegistrationModel, RegistrationModel>();

        // --- Services ---
        builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
        builder.Services.AddSingleton<FirebaseService>();
        builder.Services.AddSingleton<IBackgroundWorkerService, BackgroundWorkerService>();

        // --- Factories ---
        builder.Services.AddTransient<IStatEntryFactory, StatEntryFactory>();

        // --- Views (for DI resolution) ---
        builder.Services.AddTransient<AreasView>();
        builder.Services.AddTransient<AuthorizationView>();
        builder.Services.AddTransient<GlossaryView>();
        builder.Services.AddTransient<StatsView>();
        builder.Services.AddTransient<AboutBiobrainView>();
        builder.Services.AddTransient<TopicsView>();
        builder.Services.AddTransient<MaterialView>();
        builder.Services.AddTransient<WordView>();
        builder.Services.AddTransient<LetterView>();
        builder.Services.AddTransient<TextView>();
        builder.Services.AddTransient<AccountView>();
        builder.Services.AddTransient<DeleteAccountView>();
        builder.Services.AddTransient<SearchView>();
        builder.Services.AddTransient<FeedbackView>();
        builder.Services.AddTransient<AboutView>();
        builder.Services.AddTransient<QuestionsView>();
        builder.Services.AddTransient<ResultsView>();
        builder.Services.AddTransient<QuestionReviewView>();
        builder.Services.AddTransient<StatsListView>();
        builder.Services.AddTransient<ElementsTableView>();

        // --- DependencyService registrations (legacy code compatibility) ---
        DependencyService.Register<IErrorLog, ConsoleErrorLog>();
        DependencyService.Register<IAnalyticTracker, NoOpAnalyticTracker>();
        DependencyService.Register<IFilesPath, StubFilesPath>();
        DependencyService.Register<IProjectDataWorker, StubProjectDataWorker>();
        DependencyService.Register<ISQLite, StubSQLite>();
        DependencyService.Register<IWorkingWithFiles, StubWorkingWithFiles>();
        DependencyService.Register<IAccountDataProvider, StubAccountDataProvider>();
        DependencyService.Register<IAccountDataStoreManager, StubAccountDataStoreManager>();
        DependencyService.Register<IActionSheet, StubActionSheet>();
        DependencyService.Register<IApplicationParamsProvider, StubApplicationParamsProvider>();
        DependencyService.Register<IClassKit, NoOpClassKit>();
        DependencyService.Register<ICountriesService, StubCountriesService>();
        DependencyService.Register<IDecryption, NoOpDecryption>();
        DependencyService.Register<IEmailSender, NoOpEmailSender>();
        DependencyService.Register<IOrientationService, NoOpOrientationService>();
        DependencyService.Register<IRateApp, StubRateApp>();
        DependencyService.Register<IZipResource, NoOpZipResource>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        ServiceDescriptors = builder.Services;
        return builder.Build();
    }
}

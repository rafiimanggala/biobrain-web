using BioBrain.Factories.Implementation;
using BioBrain.Factories.Interfaces;
using BioBrain.Services.Implementations;
using BioBrain.Services.Interfaces;
using BioBrain.ViewModels.Implementation;
using BioBrain.ViewModels.Interfaces;
using CustomControls.Interfaces;
using CustomControls.LayoutControls.ChemicalElements;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Implementations;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI

namespace BioBrain.Helpers
{
    public static class UnityContainerHelper
    {
        public static void RegisterDependencies()
        {
            App.Container.RegisterType<IAreasOfStudyViewModel, AreasOfStudyViewModel>();
            App.Container.RegisterType<IAreaViewModel, AreaViewModel>();
            App.Container.RegisterType<ITopicsViewModel, TopicsViewModel>();
            App.Container.RegisterType<ITopicViewModel, TopicViewModel>();
            App.Container.RegisterType<IAreasRepository, AreasRepository>();
            App.Container.RegisterType<ITopicsRepository, TopicsRepository>();
            App.Container.RegisterType<IDoneAreasRepository, DoneAreasRepository>();
            App.Container.RegisterType<IDoneTopicsRepository, DoneTopicsRepository>();
            App.Container.RegisterType<IDoneAreaModel, DoneAreaModel>();
            App.Container.RegisterType<IDoneTopicModel, DoneTopicModel>();
            App.Container.RegisterType<ILevelsViewModel, LevelsViewModel>();
            App.Container.RegisterType<ILevelViewModel, LevelViewModel>();
            App.Container.RegisterType<ILevelTypesRepository, LevelTypesRepository>();
            App.Container.RegisterType<IMaterialsViewModel, MaterialViewModel>();
            App.Container.RegisterType<IMaterialsRepository, MaterialsRepository>();
            App.Container.RegisterType<IDoneMaterialsRepository, DoneMaterialsRepository>();
            App.Container.RegisterType<IQuestionViewModel, QuestionViewModel>();
            App.Container.RegisterType<IQuestionsViewModel, QuestionsViewModel>();
            App.Container.RegisterType<IQuestionsRepository, QuestionsRepository>();
            App.Container.RegisterType<IAnswersRepository, AnswersRepository>();
            App.Container.RegisterType<IBaseAnswerViewModel, BaseAnswerViewModel>();
            App.Container.RegisterType<IDoneQuestionsRepository, DoneQuestionRepository>();
            App.Container.RegisterType<IResultViewModel, ResultViewModel>();
            App.Container.RegisterType<IResultElement, QuestionResultViewModel>();
            App.Container.RegisterType<IGlossaryRepository, GlossaryRepository>();
            App.Container.RegisterType<IGlossaryViewModel, GlossaryViewModel>();
            App.Container.RegisterType<ILetterViewModel, LetterViewModel>();
            App.Container.RegisterType<IWordViewModel, WordViewModel>();
            App.Container.RegisterType<ITextViewModel, TextViewModel>();
            App.Container.RegisterType<IQuestionReviewViewModel, QuestionReviewViewModel>();
            App.Container.RegisterType<IAboutViewModel, AboutViewModel>();
            App.Container.RegisterType<IAccountViewModel, AccountViewModel>();
            App.Container.RegisterType<IAccountModel, AccountModel>();
            App.Container.RegisterType<IAccountRepository, AccountRepository>();
            App.Container.RegisterType<IFirebaseRepository, FirebaseRepository>();
            App.Container.RegisterType<IDatabaseRepository, DatabaseRepository>();
            App.Container.RegisterType<IFirebaseService, FirebaseService>();
            App.Container.RegisterType<IFirebasePurchaseModel, FirebasePurchaseModel>();
            App.Container.RegisterType<IPurchaseInfoViewModel, PurchaseInfoViewModel>();
            App.Container.RegisterType<IFeedbackViewModel, FeedbackViewModel>();
            App.Container.RegisterType<IAboutBiobrainViewModel, AboutBiobrainViewModel>();
            App.Container.RegisterType<IElementViewModel, ElementViewModel>();
            App.Container.RegisterType<IElemetsTablePageViewModel, ElementsTablePageViewModel>();
            App.Container.RegisterType<ISendReviewViewModel, SendReviewViewModel>();
            App.Container.RegisterType<ISearchViewModel, SearchViewModel>();
            App.Container.RegisterType<ISearchResultViewModel, SearchResultViewModel>();
            App.Container.RegisterType<IStatsViewModel, StatsViewModel>();
            App.Container.RegisterType<IStatEntryFactory, StatEntryFactory>();
            App.Container.RegisterType<IStatListViewModel, StatListViewModel>();
            App.Container.RegisterType<IBackgroundWorkerService, BackgroundWorkerService>();
            App.Container.RegisterType<IRegistrationModel, RegistrationModel>();
            App.Container.RegisterType<IAuthorizationViewModel, AuthorizationViewModel>();
            App.Container.RegisterType<IDataUpdateViewModel, DataUpdateViewModel>();
            App.Container.RegisterType<IDeleteAccountViewModel, DeleteAccountViewModel>();
        }
    }
}

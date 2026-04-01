using System;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.AppResources;
using BioBrain.Views;
using BioBrain.ViewModels.Interfaces;
using Common;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.Extensions
{
    public static class NavigationExtension
    {
        public static async Task MoveToHome(this INavigation navigation)
        {
            if(navigation.NavigationStack.Count == 1) return;
            await navigation.PopToRootAsync(false);
            GC.Collect();
        }

        public static async Task MoveToAboutBiobrain(this INavigation navigation)
        {
            if(navigation.NavigationStack.Last() is AboutBiobrainView) return;
            var view = App.Container.Resolve<AboutBiobrainView>();
            await navigation.PushAsync(view, false);
            RemoveOfSameModule(navigation, view);
        }

        public static async Task MoveToPeriodicTable(this INavigation navigation)
        {
            if (navigation.NavigationStack.Last() is ElementsTableView) return;
            var view = App.Container.Resolve<ElementsTableView>();
            await navigation.PushAsync(view, false);
            RemoveOfSameModule(navigation, view);
        }

        public static async Task MoveToStats(this INavigation navigation)
        {
            if (navigation.NavigationStack.Last() is StatsView) return;
            var view = App.Container.Resolve<StatsView>();
            await navigation.PushAsync(view, false);
            RemoveOfSameModule(navigation, view);
        }

        public static async Task MoveToGlossary(this INavigation navigation)
        {
            if (navigation.NavigationStack.Last() is GlossaryView) return;
            var view = App.Container.Resolve<GlossaryView>();
            await navigation.PushAsync(view, false);
            RemoveOfSameModule(navigation, view);
        }

        public static async Task MoveToAuthorization(this INavigation navigation)
        {
            if (navigation.NavigationStack.Last() is AuthorizationView) return;
            var view = App.Container.Resolve<AuthorizationView>();
            await navigation.PushAsync(view, false);
            var toRemove = navigation.NavigationStack.Where(p => p != view).ToList();
            foreach (var p in toRemove)
            {
                try { navigation.RemovePage(p); }
                catch (Exception ex) { Console.WriteLine($"RemovePage error: {ex.Message}"); }
            }
        }

        public static void RemoveOfSameModule(this INavigation navigation, IBaseView view)
        {
            var toRemove = navigation.NavigationStack.Where(p =>
                ((IBaseView) p)?.MenuItem == view.MenuItem && p != view).ToList();
            foreach (var p in toRemove)
            {
                try { navigation.RemovePage(p); }
                catch (Exception ex) { Console.WriteLine($"RemovePage error: {ex.Message}"); }
            }
        }

        public static void DeleteOfType(this INavigation navigation, Type type)
        {
            var pagesToRemove = navigation.NavigationStack.Where(page => page.GetType() == type).ToList();

            foreach (var page in pagesToRemove)
            {
                try { navigation.RemovePage(page); }
                catch (Exception ex) { Console.WriteLine($"RemovePage error: {ex.Message}"); }
            }
            GC.Collect();
        }

        public static async Task GoToMaterial(this INavigation navigation, int id)
        {
            await navigation.PushAsync(App.Container.Resolve<MaterialView>(new ParameterOverride("materialID", id)), false);
        }

        public static async Task MoveBack(this INavigation navigation)
        {
            await navigation.PopAsync(false);
            GC.Collect();
        }

        public static async Task MoveToWord(this INavigation navigation, int wordID)
        {
            await navigation.PushAsync(App.Container.Resolve<WordView>(new ParameterOverride("wordID", wordID)), false);
        }

        public static async Task MoveToAbout(this INavigation navigation)
        {
            await navigation.PushAsync(App.Container.Resolve<TextView>(new ParameterOverride("fileName", FileNames.HelpPage), 
                new ParameterOverride("pageHeader", StringResource.AboutViewHeaderString)), false);
        }

        public static async Task MoveToDeleteAccount(this INavigation navigation)
        {
            await navigation.PushAsync(App.Container.Resolve<DeleteAccountView>(), false);
        }

        public static async Task MoveToTextView(this INavigation navigation, string fileName, string pageHeader)
        {
            await navigation.PushAsync(App.Container.Resolve<TextView>(new ParameterOverride("fileName", fileName),
                new ParameterOverride("pageHeader", pageHeader)), false);
        }

        public static async Task MoveToAccount(this INavigation navigation)
        {
            await navigation.PushAsync(App.Container.Resolve<AccountView>(), false);
        }

        public static async void MoveToAccountWithHistoryRestore(this INavigation navigation)
        {
            var page = App.Container.Resolve<AccountView>();
            await navigation.PushAsync(page, false);
            navigation.InsertPageBefore(App.Container.Resolve<AboutBiobrainView>(), page);
        }

        public static async Task MoveToAreasAndInitUpdate(this INavigation navigation)
        {
            try
            {
	            App.IsPurchaseMade = true;
                await navigation.PopToRootAsync(false);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task MoveToAreasAfterAuth(this INavigation navigation)
        {
            try
            {
                if (!(navigation.NavigationStack.Last() is AuthorizationView)) return;
                var view = App.Container.Resolve<AreasView>();
                await navigation.PushAsync(view, false);
                var toRemove = navigation.NavigationStack.Where(p => p != view).ToList();
                foreach (var p in toRemove)
                {
                    try { navigation.RemovePage(p); }
                    catch (Exception ex) { Console.WriteLine($"RemovePage error: {ex.Message}"); }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task MoveToTopicFromQuestion(this INavigation navigation)
        {
            navigation.DeleteOfType(typeof(MaterialView));
            await navigation.MoveBack();
            GC.Collect();
        }

        public static async Task MoveToQuestionReview(this INavigation navigation, int questionID)
        {
            var view = App.Container.Resolve<QuestionReviewView>(new ParameterOverride("questionID", questionID));
            await navigation.PushAsync(view, false);
        }

        public static async Task MoveToResults(this INavigation navigation, int materialID)
        {
            var view = App.Container.Resolve<ResultsView>(new ParameterOverride("materialID", materialID));
            await navigation.PushAsync(view, false);
            navigation.DeleteOfType(typeof(QuestionsView));
        }

        public static async Task MoveToQuestions(this INavigation navigation, int topicID, int materialID)
        {
            var view = App.Container.Resolve<QuestionsView>(new ParameterOverride("topicID", topicID), new ParameterOverride("materialID", materialID));
            await navigation.PushAsync(view, false);
        }

        public static async Task MoveToTopics(this INavigation navigation, int areaId)
        {
            await navigation.PushAsync(App.Container.Resolve<TopicsView>(string.Empty, new ParameterOverride("areaOfStudyId", areaId)), false);
        }

        public static async Task MoveToUpdates(this INavigation navigation, bool forceUpdate = false)
        {
            try
            {
                // Resolve ViewModel with the bool parameter first, then resolve View
                var vm = App.Container.Resolve<IDataUpdateViewModel>(string.Empty, new ParameterOverride("isForceDownload", forceUpdate));
                var view = new DataUpdateView(vm);
                await navigation.PushAsync(view);
            }
            catch (Exception e)
            {
                Console.WriteLine($"MoveToUpdates error: {e.Message}");
            }
        }

        public static async Task HandleInitialNavigation(this INavigation navigation)
        {
            var path = App.InitialNavigationPath;
            App.InitialNavigationPath = null;
            if(path == null) return;

            switch (path.Count)
            {
                case 1:
                    if(int.TryParse(path[0], out var areaId))
                        await MoveToTopics(navigation, areaId);
                    break;
                case 3:
                    if (int.TryParse(path[2], out var materialId))
                        await GoToMaterial(navigation, materialId);
                    break;
                default: return;
            }
        }
    }
}
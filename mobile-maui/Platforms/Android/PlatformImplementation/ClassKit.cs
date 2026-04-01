using System.Collections.Generic;
using System.Threading.Tasks;
using BioBrain.Interfaces;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IClassKit, ClassKit>();
namespace BioBrain.Platforms.Android.PlatformImplementation
{
    /// <summary>
    /// Android stub for ClassKit (ClassKit is iOS-only, Apple Schoolwork integration).
    /// All methods are no-ops on Android.
    /// </summary>
    public class ClassKit : IClassKit
    {
        public Task SetupClassKitContexts(List<AreaModel> areasToAdd, List<TopicModel> topicsToAdd,
            List<MaterialModel> materialsToAdd, List<LevelTypeModel> levelTypes)
        {
            return Task.CompletedTask;
        }

        public void SetupClassKitContexts(List<IAreaModel> areasToAdd, List<ITopicModel> topicsToAdd,
            List<IMaterialModel> materialsToAdd, List<ILevelTypeModel> levelTypes)
        {
        }

        public void StartActivity(List<string> identifierPath, bool asNew = false)
        {
        }

        public void UpdateProgress(string[] identifierPath, double progress)
        {
        }

        public void AddScore(string[] identifierPath, double score, double maxScore, string title, bool primary = false)
        {
        }

        public void StopActivity(string[] identifierPath)
        {
        }
    }
}

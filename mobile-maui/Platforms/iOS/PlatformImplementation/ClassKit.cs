using System;
using System.Collections.Generic;
using System.Linq;
using BioBrain.Interfaces;
using ClassKit;
using CoreFoundation;
using DAL.Models.Interfaces;
using UIKit;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IClassKit, ClassKit>();
namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    public class ClassKit : IClassKit
    {
        /// <summary>
        /// Declare content
        /// </summary>
        public void SetupClassKitContexts(List<IAreaModel> areasToAdd, List<ITopicModel> topicsToAdd,
            List<IMaterialModel> materialsToAdd, List<ILevelTypeModel> levelTypes)
        {
            var library = new ContentLibrary();
            library.SetupClassKit();

            foreach (var areaModel in areasToAdd)
            {
                var topicsModels = topicsToAdd.Where(x => x.AreaID == areaModel.AreaID);
                foreach (var topicModel in topicsModels)
                {
                    var materialsModels = materialsToAdd.Where(x => x.TopicID == topicModel.TopicID);
                    foreach (var materialModel in materialsModels)
                    {
                        CLSDataStore.Shared.MainAppContext.FindDescendantMatching(
                            new[]
                            {
                                areaModel.AreaID.ToString(),
                                topicModel.TopicID.ToString(),
                                materialModel.MaterialID.ToString()
                            },
                            (context, error) =>
                            {
                                if (error == null) return;
                                Console.WriteLine($"Class Kit Declare: {error.LocalizedDescription}");
                                DispatchQueue.MainQueue.DispatchAsync(() =>
                                {
                                    var alert = UIAlertController.Create("Error",
                                        $"Class Kit Declare: {error.LocalizedDescription}",
                                        UIAlertControllerStyle.Alert);
                                    alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, null));
                                });
                            }
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Start activity
        /// </summary>
        public void StartActivity(List<string> identifierPath, bool asNew = false)
        {
            // ClassKit activity start - commented out in original, preserved for future implementation
        }

        /// <summary>
        /// Update progress
        /// </summary>
        public void UpdateProgress(string[] identifierPath, double progress)
        {
            // ClassKit progress update - commented out in original, preserved for future implementation
        }

        /// <summary>
        /// Add score
        /// </summary>
        public void AddScore(string[] identifierPath, double score, double maxScore, string title, bool primary = false)
        {
            // ClassKit score - commented out in original, preserved for future implementation
        }

        /// <summary>
        /// Stops the current context and its activity.
        /// </summary>
        public void StopActivity(string[] identifierPath)
        {
            // ClassKit stop activity - commented out in original, preserved for future implementation
        }
    }
}

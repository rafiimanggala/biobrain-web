using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace BioBrain.Interfaces
{
    public interface IClassKit
    {
        void SetupClassKitContexts(List<IAreaModel> areasToAdd, List<ITopicModel> topicsToAdd, List<IMaterialModel> materialsToAdd, List<ILevelTypeModel> levelTypes);


        /// <summary>
        /// Start activity
        /// </summary>
        /// <param name="identifierPath"></param>
        /// <param name="asNew"></param>
        void StartActivity(List<string> identifierPath, bool asNew = false);

        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="identifierPath"></param>
        /// <param name="progress"></param>
        void UpdateProgress(string[] identifierPath, double progress);

        /// <summary>
        /// Add score
        /// </summary>
        /// <param name="identifierPath"></param>
        /// <param name="score"></param>
        /// <param name="maxScore"></param>
        /// <param name="title"></param>
        /// <param name="primary"></param>
        void AddScore(string[] identifierPath, double score, double maxScore, string title,
            bool primary = false);

        /// <summary>
        /// Stops the current context and its activity.
        /// </summary>
        /// <param name="identifierPath"></param>
        void StopActivity(string[] identifierPath);
    }
}
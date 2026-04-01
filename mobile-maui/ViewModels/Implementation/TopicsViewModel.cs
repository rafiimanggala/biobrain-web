using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    class TopicsViewModel : BasePurchasableViewModel, ITopicsViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        public ObservableCollection<ITopicViewModel> Topics { get; set; }
        public string AreaOfStudyName { get; }
        public string AreaOfStudyIcon { get; set; }
        //Used through binding
        public bool IsTableVisible => Settings.IsPeriodicTableVisible;

        private readonly int areaOfStudyId;
        private readonly ITopicsRepository topicsRepository;
        private readonly IDoneTopicsRepository doneTopicsRepository;
        private readonly IMaterialsRepository materialsRepository;

        public TopicsViewModel(
            int areaOfStudyId,
            ITopicsRepository topicsRepository,
            IDoneTopicsRepository doneTopicsRepository,
            IAreasRepository areasRepository,
            IMaterialsRepository materialsRepository)
        {
            this.topicsRepository = topicsRepository;
            this.areaOfStudyId = areaOfStudyId;
            this.doneTopicsRepository = doneTopicsRepository;
            this.materialsRepository = materialsRepository;
            Topics = new ObservableCollection<ITopicViewModel>();
            try
            {
                var area = areasRepository.GetByID(areaOfStudyId);
                AreaOfStudyName = area.AreaName.ToUpper();
                AreaOfStudyIcon = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, area.Image);
            }
            catch (Exception ex)
            {
                logger.Log($"TopicsViewModel constructor error: {ex.Message}");
                AreaOfStudyName = string.Empty;
            }
            //GetTopics();
        }

        public void GetTopics()
        {
            try
            {
                var models = topicsRepository.GetForArea(areaOfStudyId);
                Topics.Clear();

                foreach (var topicModel in models.OrderBy(m => m.TopicOrder))
                {
                    var topic = App.Container.Resolve<ITopicViewModel>(new ParameterOverride("model", topicModel));
                    topic.IsDone = doneTopicsRepository.IsTopicDone(topicModel.TopicID);

                    topic.CompletionString = $"{materialsRepository.CountDoneForTopic(topicModel.TopicID)}/{materialsRepository.CountAllForTopic(topicModel.TopicID)}";
                    Topics.Add(topic);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"TopicsViewModel GetTopics error: {ex.Message}");
            }
        }
    }
}

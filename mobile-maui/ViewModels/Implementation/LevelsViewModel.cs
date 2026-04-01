using System.Collections.Generic;
using System.IO;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    class LevelsViewModel : ILevelsViewModel
    {
        public List<ILevelViewModel> Levels { get; set; }

        public string TopicName { get; }

        public string TopicIconPath { get; }

        public string TopicImagePath { get; }

        private readonly int topicID;
        private readonly ILevelTypesRepository levelTypesRepository;
        private readonly IDoneMaterialsRepository doneMaterilasRepository;
        private readonly IMaterialsRepository materialsRepository;

        public LevelsViewModel(int topicID,  ILevelTypesRepository levelTypesRepository, IDoneMaterialsRepository doneMaterilasRepository, ITopicsRepository topicsRepository, IMaterialsRepository materialsRepository)
        {
            this.topicID = topicID;
            this.levelTypesRepository = levelTypesRepository;
            this.doneMaterilasRepository = doneMaterilasRepository;
            this.materialsRepository = materialsRepository;
            var topic = topicsRepository.GetByID(topicID);
            TopicName = topic.TopicName.ToUpper();
            TopicIconPath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topic.Image);
            TopicImagePath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topic.BackgroundImage);
            GetLevels();
        }

        private void GetLevels()
        {
            var models = levelTypesRepository.GetAll();
            Levels = new List<ILevelViewModel>();

            foreach (var levelModel in models)
            {
                var materialModel = materialsRepository.GetForLevelAndTopic(levelModel.LevelTypeID, topicID);
                var level = App.Container.Resolve<ILevelViewModel>(new ParameterOverride("materialModel", materialModel), new ParameterOverride("typeModel", levelModel));
                level.IsDone = doneMaterilasRepository.IsMaterialDone(materialModel.MaterialID);
                Levels.Add(level);
            }
        }
    }
}

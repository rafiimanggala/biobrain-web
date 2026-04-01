using System.IO;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Common;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{

    class TopicViewModel : ITopicViewModel, ICellViewModel
    {
        private int FirstLevelID => levelTypesRepository.GetFirstLevelId();
        public int TopicID => model.TopicID;
        public string Name => Settings.TopicNamesExtensions.Contains(model.TopicName) ? model.TopicName : model.TopicName.ToUpper();
        public string IconPath => Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, model.Image);
        public string DarkIconPath => Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, model.DarkImage);
        public bool IsDone { get; set; }
        public string CompletionString { get; set; }
        public int MaterialID => materialsRepository.GetForLevelAndTopic(FirstLevelID, TopicID)?.MaterialID ?? -1;

        private readonly ITopicModel model;
        private readonly IMaterialsRepository materialsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;

        public TopicViewModel(ITopicModel model, IMaterialsRepository materialsRepository, ILevelTypesRepository levelTypesRepository)
        {
            this.model = model;
            this.materialsRepository = materialsRepository;
            this.levelTypesRepository = levelTypesRepository;
        }
        
    }
}

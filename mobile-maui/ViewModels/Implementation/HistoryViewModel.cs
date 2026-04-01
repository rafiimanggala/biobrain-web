//using System.Collections.Generic;
//using System.ComponentModel;
//using BioBrain.ViewModels.Interfaces;
//using DAL.Repositorys.Interfaces;
//// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

//namespace BioBrain.ViewModels.Implementation
//{
//    class HistoryViewModel : IHistoryViewModel
//    {
//        private readonly IAreasRepository areasRepository;
//        private readonly ITopicsRepository topicsRepository;
//        private readonly ILevelTypesRepository levelTypesRepository;
//        private readonly IMaterialsRepository materialsRepository;

//        public HistoryViewModel(IAreasRepository areasRepository, ITopicsRepository topicsRepository, ILevelTypesRepository levelTypesRepository, IMaterialsRepository materialsRepository)
//        {
//            this.areasRepository = areasRepository;
//            this.topicsRepository = topicsRepository;
//            this.levelTypesRepository = levelTypesRepository;
//            this.materialsRepository = materialsRepository;

//            InitializeData();
//        }

//        private void InitializeData()
//        {
//            Areas = new List<IAreaViewModel>();
//            Topics = new List<ITopicViewModel>();

//            var areasModels = areasRepository.GetAll();
//            areasModels.ForEach(a => Areas.Add(new AreaViewModel(a)));

//            var topicModels = topicsRepository.GetAll();
//            topicModels.ForEach(t => Topics.Add(new TopicViewModel(t, materialsRepository, levelTypesRepository)));

//            foreach (var area in areasModels)
//            {
//                var topics = topicModels.FindAll(t => t.AreaID == area.AreaID);
//                foreach (var topic in topics)
//                {
//                    var materials = materialsRepository.GetForTopic(topic.TopicID);
//                    foreach (var material in materials)
//                    {
//                        var level = levelTypesRepository.GetByID(material.LevelTypeID);

//                    }
//                }
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public List<IHistoryEntryViewModel> Entries { get; set; }

//        public List<IAreaViewModel> Areas { get; set; }

//        public List<ITopicViewModel> Topics { get; set; }

//        public int SelectedAreaID { get; set; }

//        public int SelectedTopicID { get; set; }

//        public void FilterElements()
//        {
//            throw new System.NotImplementedException();
//        }

//        private void NotifyPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}

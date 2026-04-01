using System;
using System.Collections.ObjectModel;
using System.Linq;
using BioBrain.Factories.Interfaces;
using BioBrain.Helpers;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;
// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

namespace BioBrain.ViewModels.Implementation
{
    class StatListViewModel : IStatListViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IDoneMaterialsRepository doneMaterialsRepository;
        private readonly IMaterialsRepository materialsRepository;
        private readonly IStatEntryFactory statEntryFactory;
        private readonly ITopicsRepository topicsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;

        public string ViewHeader => StringResource.StatsHeaderString.ToUpper();
        public ObservableCollection<IStatEntryViewModel> StatsList { get; private set; } = new ObservableCollection<IStatEntryViewModel>();

        public StatListViewModel(IDoneMaterialsRepository doneMaterialsRepository, IMaterialsRepository materialsRepository, IStatEntryFactory statEntryFactory,
            ITopicsRepository topicsRepository, ILevelTypesRepository levelTypesRepository)
        {
            this.doneMaterialsRepository = doneMaterialsRepository;
            this.materialsRepository = materialsRepository;
            this.statEntryFactory = statEntryFactory;
            this.topicsRepository = topicsRepository;
            this.levelTypesRepository = levelTypesRepository;
            //GetData();
        }

        public void GetData()
        {
            try
            {
                var doneMaterials = doneMaterialsRepository.GetAll();
                StatsList.Clear();

                foreach (var materialModel in doneMaterials.Where(dm => dm.Adge < 1).OrderByDescending(dm => dm.Date))
                {
                    var material = materialsRepository.GetByID(materialModel.MaterialID);
                    var topic = topicsRepository.GetByID(material.TopicID);
                    var levels = levelTypesRepository.GetAll();
                    var levelName = (levels.OrderBy(x => x.LevelTypeID).ToList().FindIndex(x => x.LevelTypeID == material.LevelTypeID) + 1).ToString();
                    StatsList.Add(statEntryFactory.Get(topic.TopicName, $"{levelName}", materialModel.Date.ToString("d"), $"{(materialModel.Score * 10)}%", topic.TopicID, material.MaterialID));
                }
            }
            catch (Exception ex)
            {
                logger.Log($"StatListViewModel GetData error: {ex.Message}");
            }
        }

        public void ToggleSendMode()
        {
            var resultsToSend = StatsList?.Where(x => x.IsNeedToSend).ToList();
            if (resultsToSend == null || !resultsToSend.Any()) return;

            EmailHelper.SendResultsEmail(resultsToSend, logger);
        }
    }
}

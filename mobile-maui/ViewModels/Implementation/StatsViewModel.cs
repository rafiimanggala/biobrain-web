using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
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
    public class StatsViewModel : IStatsViewModel, INotifyPropertyChanged
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IDoneMaterialsRepository doneMaterialsRepository;
        private readonly IMaterialsRepository materialsRepository;
        private readonly IStatEntryFactory statEntryFactory;
        private readonly ITopicsRepository topicsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;

        public StatsViewModel(IDoneMaterialsRepository doneMaterialsRepository, IMaterialsRepository materialsRepository, IStatEntryFactory statEntryFactory,
            ITopicsRepository topicsRepository, ILevelTypesRepository levelTypesRepository)
        {
            this.doneMaterialsRepository = doneMaterialsRepository;
            this.materialsRepository = materialsRepository;
            this.statEntryFactory = statEntryFactory;
            this.topicsRepository = topicsRepository;
            this.levelTypesRepository = levelTypesRepository;
        }

        public string ViewHeader => StringResource.StatsSummaryHeaderString.ToUpper();
        public double AverageQuizRating { get; private set; }
        public double QuizzesCompleted { get; private set; }
        public ObservableCollection<IStatEntryViewModel> StatsList { get; } = new ObservableCollection<IStatEntryViewModel>();

        public void GetData()
        {
            try
            {
                StatsList.Clear();
                var doneMaterials = doneMaterialsRepository.GetAll().Where(dm => dm.Adge < 1).ToList();
                var materialsCount = materialsRepository.GetAll().Count;
                if (doneMaterials.Count < 1)
                {
                    AverageQuizRating = 0;
                    QuizzesCompleted = 0;
                    return;
                }

                AverageQuizRating = Math.Round((double)(doneMaterials.Select(dm => dm.Score).Aggregate((s1, s2) => s1 + s2) * 10) /
                                    doneMaterials.Count);
                QuizzesCompleted = Math.Round((double)doneMaterials.Count / materialsCount * 100);

                foreach (var materialModel in doneMaterials.OrderByDescending(dm=>dm.Date).Take(5))
                {
                    var material = materialsRepository.GetByID(materialModel.MaterialID);
                    var topic = topicsRepository.GetByID(material.TopicID);
                    var levels = levelTypesRepository.GetAll();
                    var levelName = (levels.OrderBy(x => x.LevelTypeID).ToList().FindIndex(x => x.LevelTypeID == material.LevelTypeID) + 1).ToString();
                    var entry = statEntryFactory.Get(topic.TopicName, levelName, materialModel.Date.ToString("d"),
                        $"{(materialModel.Score * 10)}%", topic.TopicID, material.MaterialID);
                    StatsList.Add(entry);
                    logger.Log($"StatsView - Material: {entry.Topic} {entry.Level}; Score: {entry.Score}; Date: {materialModel.Date:dd/MM/yyyy HH:mm:ss}");
                }
                OnPropertyChanged(nameof(AverageQuizRating));
                OnPropertyChanged(nameof(QuizzesCompleted));
                OnPropertyChanged(nameof(StatsList));
            }
            catch (Exception ex)
            {
                logger.Log($"StatsViewModel GetData error: {ex.Message}");
            }
        }

        public void ToggleSendMode()
        {
            var resultsToSend = StatsList?.Where(x => x.IsNeedToSend).ToList();
            if (resultsToSend == null || !resultsToSend.Any()) return;

            EmailHelper.SendResultsEmail(resultsToSend, logger);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Common.Interfaces;
using CustomControls;
using CustomControls.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class ResultViewModel : IResultViewModel, INotifyPropertyChanged
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();

        public List<IResultElement> Questions { get; set; }

        public int Percent { get; set; }

        public int Progress { get; set; }

        public bool ShowProgress { get; set; }

        public string HeaderString { get; set; }

        public string HeaderString2 { get; set; }

        public bool HeaderString2Visible => !string.IsNullOrEmpty(HeaderString2);

        public int AreaID { get; set; }

        public int NextMaterialID { get; set; }

        public int NextTopicMaterialID { get; set; }

        public string TopicIconPath { get; set; }

        public bool IsNextLevelVisible { get; set; }

        public bool IsNextTopicVisible { get; set; }

        public string AreaName { get; set; }
        public string TopicName { get; set; }
        public string LevelName { get; set; }

        private readonly int materialID;
        private readonly IQuestionsRepository questionsRepository;
        private readonly IDoneQuestionsRepository doneQuestionsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;
        private readonly ITopicsRepository topicsRepository;
        private readonly IMaterialsRepository materialsRepository;
        private readonly IDoneMaterialsRepository doneMaterialsRepository;
        private readonly IAreasRepository areasRepository;
        private readonly IAccountDataStoreManager accountDataStoreManager = DependencyService.Get<IAccountDataStoreManager>();

        public ResultViewModel(int materialID, IAreasRepository areasRepository, IMaterialsRepository materialsRepository, IQuestionsRepository questionsRepository, IDoneQuestionsRepository doneQuestionsRepository, ILevelTypesRepository levelTypesRepository, ITopicsRepository topicsRepository, IDoneMaterialsRepository doneMaterialsRepository)
        {
            this.materialID = materialID;
            this.areasRepository = areasRepository;
            this.questionsRepository = questionsRepository;
            this.doneQuestionsRepository = doneQuestionsRepository;
            this.levelTypesRepository = levelTypesRepository;
            this.topicsRepository = topicsRepository;
            this.materialsRepository = materialsRepository;
            this.doneMaterialsRepository = doneMaterialsRepository;

            try { SetData(); }
            catch (Exception ex) { logger.Log($"ResultViewModel constructor error: {ex.Message}"); }
        }

        private void SetData()
        {
            Questions = new List<IResultElement>();
            var questions = questionsRepository.GetByMaterial(materialID).OrderBy(q => q.QuestionOrder).ToList();
            double correctQuestions = 0;
            foreach (var question in questions)
            {
                var doneQuestion = doneQuestionsRepository.GetByQuestionID(question.QuestionID);
                if (doneQuestion == null)
                    continue;
                Questions.Add(new QuestionResultViewModel { Name = "Q"+question.QuestionOrder, Color = doneQuestion.IsCorrect ? CustomColors.DarkGreen : CustomColors.Red, QuestionID = question.QuestionID});
                if (doneQuestion.IsCorrect) correctQuestions++;
            }

            Percent = (int)(correctQuestions/ questions.Count * 100);
            var oldScore = doneMaterialsRepository.GetEntriesForMaterial(materialID).FirstOrDefault(m => m.Adge == 1);
            ShowProgress = oldScore != null;
            if (ShowProgress)
            {
                Progress = Percent - oldScore.Score*10;
            }

            var material = materialsRepository.GetByID(materialID);

            if (material.LevelTypeID == 3)
                NextMaterialID = -1;
            else
                NextMaterialID = materialsRepository.GetForLevelAndTopic(material.LevelTypeID + 1, material.TopicID)?.MaterialID ?? -1;

            var topic = topicsRepository.GetByID(material.TopicID);
            TopicName = topic.TopicName;

            if (topic.TopicOrder == topicsRepository.CountForArea(topic.AreaID))
                NextTopicMaterialID = -1;
            else
            {
                var nextTopicID = topicsRepository.GetByAreaAndOrder(topic.AreaID, topic.TopicOrder + 1)?.TopicID ?? -1;
                if (nextTopicID == -1)
                {
                    NextTopicMaterialID = -1;
                }
                else
                    NextTopicMaterialID = materialsRepository.GetForLevelAndTopic(levelTypesRepository.FirstLevelID, nextTopicID)?.MaterialID ?? -1;
            }

            var level = levelTypesRepository.GetByID(material.LevelTypeID);
            LevelName = level.LevelShortName;
            AreaID = topic.AreaID;
            HeaderString = topic.TopicName.Length <= 27
                ? level.LevelName.ToUpper() + ": " +
                  topic.TopicName.ToUpper()
                : $"{level.LevelName.ToUpper()}:";
            HeaderString2 = topic.TopicName.Length <= 27 ? string.Empty : topic.TopicName.ToUpper();

            TopicIconPath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topic.Image);

            IsNextLevelVisible = material.LevelTypeID != 3 && !Settings.IsDemo;
            IsNextTopicVisible = (topicsRepository.CountForArea(AreaID) != topic.TopicID && !Settings.IsDemo) || (Settings.IsDemo && topic.TopicID != 3);
            var area = areasRepository.GetByID(topic.AreaID);
            AreaName = area.AreaName;

            var currentQuestion = doneMaterialsRepository.GetEntriesForMaterial(materialID).FirstOrDefault(m => m.Adge == 0);
            logger.Log($"ResultsView - Material: {levelTypesRepository.GetByID(material.LevelTypeID).LevelName}: {topic.TopicName}; Score: {currentQuestion?.Score}; Date: {currentQuestion?.Date:dd/MM/yyyy HH:mm:ss}");
        }



        public void SaveRateResult(RateResult result)
        {
            accountDataStoreManager.SaveUserRateResult(result);
            App.IsRateShown = true;
        }

        public bool IsRateNeed()
        {
            if (App.IsRateShown)
            {
                return false;
            }
            var lastResult = accountDataStoreManager.GetUserRateResult();
            switch (lastResult)
            {
                case RateResult.DontRemind:
                case RateResult.Review:
                    return false;
                case RateResult.NextTime:
                    var number = accountDataStoreManager.GetEnterNumber();
                    if (number < 5) accountDataStoreManager.SaveEnterNumber(number + 1);
                    if (number == 1 || number == 5) return true;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        
        #region PropertyChanged iplementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // PropertyChanged iplementation

    }
}
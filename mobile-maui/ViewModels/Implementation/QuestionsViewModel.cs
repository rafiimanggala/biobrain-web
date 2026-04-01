using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.AppResources;
using BioBrain.Helpers;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class QuestionsViewModel : BasePurchasableViewModel, IQuestionsViewModel, INotifyPropertyChanged
    {
        private readonly IErrorLog _logger = DependencyService.Get<IErrorLog>();
        public int TopicID { get; set; }

        public string AreaName { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public string TopicIconPath { get; set; } = string.Empty;

        public int MaterialID { get; set; }

        public List<IQuestionViewModel> Questions { get; set; } = new List<IQuestionViewModel>();

        public int CurrentQuestionIndex { get; set; }

        public IQuestionViewModel CurrentQuestion => Questions.FirstOrDefault(x => Questions.IndexOf(x) == CurrentQuestionIndex);
        public string CurrentFilePath => CurrentQuestion?.FilePath;

        //Used through binding
        public bool IsTableVisible => Settings.IsPeriodicTableVisible;

        private readonly IQuestionsRepository questionsRepository;
        private readonly ITopicsRepository topicsRepository;
        private readonly IDoneMaterialsRepository doneMaterialsRepository;
        private readonly IDoneTopicsRepository doneTopicsRepository;
        private readonly IDoneAreasRepository doneAreasRepository;
        private readonly IMaterialsRepository materialsRepository;
        private readonly IGlossaryRepository glossaryRepository;
        private readonly IDoneQuestionsRepository doneQuestionsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;
        private readonly IAreasRepository areasRepository;
        private IClassKit classKitManager;
        private string[] identifierPath;

        public QuestionsViewModel(int topicID, int materialID, ILevelTypesRepository levelTypesRepository, IQuestionsRepository questionsRepository, ITopicsRepository topicsRepository, IDoneMaterialsRepository doneMaterialsRepository, IDoneTopicsRepository doneTopicsRepository, IDoneAreasRepository doneAreasRepository, IMaterialsRepository materialsRepository, IGlossaryRepository glossaryRepository, IAreasRepository areasRepository, IDoneQuestionsRepository doneQuestionsRepository)
        {
            this.questionsRepository = questionsRepository;
            this.topicsRepository = topicsRepository;
            this.doneMaterialsRepository = doneMaterialsRepository;
            this.doneTopicsRepository = doneTopicsRepository;
            this.doneAreasRepository = doneAreasRepository;
            this.materialsRepository = materialsRepository;
            this.glossaryRepository = glossaryRepository;
            this.doneQuestionsRepository = doneQuestionsRepository;
            this.levelTypesRepository = levelTypesRepository;
            this.areasRepository = areasRepository;

            TopicID = topicID;
            MaterialID = materialID;
        }

        private void WriteDataToFile()
        {
            if (CurrentQuestionIndex > Questions.Count-1) return;

            CurrentQuestion.FilePath = FileHelper.WriteFile(CurrentQuestion.QuestionText, FileTypes.Question);
        }

        public void GetData()
        {
            try
            {
                var topic = topicsRepository.GetByID(TopicID);
                TopicName = topic.TopicName.ToUpper();
                TopicIconPath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topic.Image);
                Questions = new List<IQuestionViewModel>();
                var questionsModels = questionsRepository.GetByMaterial(MaterialID);
                foreach (var question in questionsModels.Select(questionModel => App.Container.Resolve<IQuestionViewModel>(new ParameterOverride("model", questionModel))))
                {
                    Questions.Add(question);
                }
                Questions = Questions.OrderBy(q => q.QuestionOrder).ToList();
                DropDone();
                SetCurrentQuestion();
                AreaName = areasRepository.GetNameByID(topic.AreaID);
                LevelName = levelTypesRepository.GetByID(materialsRepository.GetByID(MaterialID).LevelTypeID).LevelName;
                identifierPath = new[] {topic.AreaID.ToString(), topic.TopicID.ToString(), MaterialID.ToString()};
                InitClassKit();
                OnPropertyChanged(nameof(TopicName));
                OnPropertyChanged(nameof(TopicIconPath));
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(CurrentFilePath));
                OnPropertyChanged(nameof(CurrentQuestionIndex));
            }
            catch (Exception ex)
            {
                _logger.Log($"QuestionsViewModel GetData error: {ex.Message}");
            }
        }

        private void InitClassKit()
        {
            classKitManager = DependencyService.Get<IClassKit>();
            classKitManager.StartActivity(identifierPath.ToList(), CurrentQuestionIndex == 0);
        }

        public void DropDone()
        {
            if(Questions.TrueForAll(q => doneQuestionsRepository.IsQuestionDone(q.QuestionID)))
                Questions.ForEach(q => doneQuestionsRepository.DeleteByQuestionId(q.QuestionID));
        }

        public void SetCurrentQuestion()
        {
            foreach (var question in Questions.Where(question => !doneQuestionsRepository.IsQuestionDone(question.QuestionID)))
            {
                CurrentQuestionIndex = question.QuestionOrder - 1;
                break;
            }
            WriteDataToFile();
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(NumberOfPages));
            OnPropertyChanged(nameof(PagerText));
        }

        public void SetMaterialDone()
        {
            var doneMaterials = doneMaterialsRepository.GetEntriesForMaterial(MaterialID) ??
                                new List<IDoneMaterialModel>();
            var score = GetScore();
            switch (doneMaterials.Count)
            {
                case 0:
                    doneMaterialsRepository.Insert(new DoneMaterialModel { MaterialID = MaterialID, Score = score, Adge = 0, Date = DateTime.UtcNow});
                    SetTopicAndAreaDone();
                    break;
                case 1:
                    doneMaterials[0].Adge = 1;
                    doneMaterialsRepository.Update(doneMaterials[0]);
                    doneMaterialsRepository.Insert(new DoneMaterialModel { MaterialID = MaterialID, Score = score, Adge = 0, Date = DateTime.UtcNow });
                    break;
                case 2:
                    doneMaterialsRepository.Remove(doneMaterials.First(m => m.Adge == 1));
                    var oldDone = doneMaterials.First(m => m.Adge == 0);
                    oldDone.Adge = 1;
                    doneMaterialsRepository.Update(oldDone);
                    doneMaterialsRepository.Insert(new DoneMaterialModel { MaterialID = MaterialID, Score = score, Adge = 0, Date = DateTime.UtcNow });
                    break;
                default: throw new InvalidDataException("Two much done materials");
            }

            classKitManager.AddScore(identifierPath, score, Questions.Count, StringResource.Score);
            classKitManager.StopActivity(identifierPath);
        }

        private void SetTopicAndAreaDone()
        {
            var materials = materialsRepository.GetForTopic(TopicID);
            var isTopickDone = materials.All(material => doneMaterialsRepository.IsMaterialDone(material.MaterialID));
            if (isTopickDone)
                doneTopicsRepository.Insert(new DoneTopicModel { TopicID = TopicID });
            var curentTopic = topicsRepository.GetByID(TopicID);
            var topics = topicsRepository.GetForArea(curentTopic.AreaID);
            var isAreaDone = topics.All(topic => doneTopicsRepository.IsTopicDone(topic.TopicID));
            if (isAreaDone)
                doneAreasRepository.Insert(new DoneAreaModel { DoneAreaID = curentTopic.AreaID });
        }

        public void ToNextQuestion()
        {
            var score = ((double) (CurrentQuestionIndex + 1)) / Questions.Count;
            CurrentQuestionIndex++;
            WriteDataToFile();
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(NumberOfPages));
            OnPropertyChanged(nameof(PagerText));
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(CurrentFilePath));
            OnPropertyChanged(nameof(CurrentQuestionIndex));
            classKitManager.UpdateProgress(identifierPath, score);
        }

        public void ToQuestion(int questionIndex)
        {
            CurrentQuestionIndex = questionIndex;
            WriteDataToFile();
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(NumberOfPages));
            OnPropertyChanged(nameof(PagerText));
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(CurrentFilePath));
            OnPropertyChanged(nameof(CurrentQuestionIndex));
        }

        public IWordViewModel GetGlossryTerm(string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var glossaryModel = glossaryRepository.GetByRef(reference);
            return glossaryModel == null ? null : App.Container.Resolve<IWordViewModel>(new ParameterOverride("wordID", glossaryModel.TermID));
        }

        public int NumberOfPages => Questions.Count;

        public int CurrentPageNumber => CurrentQuestionIndex + 1 > NumberOfPages ? NumberOfPages : CurrentQuestionIndex + 1;

        public string PagerText => $"{LevelName}: {CurrentPageNumber}/{NumberOfPages}";

        private int GetScore()
        {
            return Questions.Select(q => doneQuestionsRepository.GetByQuestionID(q.QuestionID)).Count(a => a.IsCorrect);
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.AppResources;
using BioBrain.Helpers;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using CustomControls.Interfaces;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class MaterialViewModel : BasePurchasableViewModel, IMaterialsViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        public string TopicIconPath { get; set; }
        public string AreaName { get; set; }
        public string TopicName { get; set; }
        public string LevelName { get; set; }
        public List<ILevelBarElement> Levels { get; set; }
        public string Text => model == null? "" : CustomCssStyles.MaterialsStyles + model.Text.Replace("<html>", "<html><body>").Replace("</html>", "</body></html>");
        public string FilePath { get; set; }
        public int MaterialID { get; }
        public int TopicID { get; set; }
        public int AreaID { get; set; }
        public bool IsDone { get; set; }
        public bool IsHaveQuestions { get; private set; }

        public string TakeQuizButtonText { get; private set; }

        //Used through binding
        public bool IsTableVisible => Settings.IsPeriodicTableVisible;

        private IMaterialModel model;
        private readonly IMaterialsRepository materialsRepository;
        private readonly ITopicsRepository topicsRepository;
        private readonly ILevelTypesRepository levelTypesRepository;
        private readonly IGlossaryRepository glossaryRepository;
        private readonly IDoneQuestionsRepository doneQuestionsRepository;
        private readonly IQuestionsRepository questionsRepository;
        private readonly IDoneMaterialsRepository doneMaterialsRepository;
        private readonly IAreasRepository areasRepository;

        public MaterialViewModel(int materialID, IMaterialsRepository materialsRepository, ILevelTypesRepository levelTypesRepository, ITopicsRepository topicsRepository, 
            IDoneMaterialsRepository doneMaterialsRepository, IGlossaryRepository glossaryRepository, IDoneQuestionsRepository doneQuestionsRepository, IQuestionsRepository questionsRepository,
            IAreasRepository areasRepository)
        {
            MaterialID = materialID;
            this.materialsRepository = materialsRepository;
            this.topicsRepository = topicsRepository;
            this.areasRepository = areasRepository;
            this.levelTypesRepository = levelTypesRepository;
            this.glossaryRepository = glossaryRepository;
            this.doneQuestionsRepository = doneQuestionsRepository;
            this.questionsRepository = questionsRepository;
            this.doneMaterialsRepository = doneMaterialsRepository;

            try { GetData(); }
            catch (Exception ex) { logger.Log($"MaterialViewModel constructor error: {ex.Message}"); }
        }

        public IWordViewModel GetGlossryTerm(string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var glossryModel = glossaryRepository.GetByRef(reference);
            return glossryModel == null ? null : App.Container.Resolve<IWordViewModel>(new ParameterOverride("wordID", glossryModel.TermID));
        }

        private void GetData()
        {
            model = materialsRepository.GetByID(MaterialID);
            IsDone = IsAllQuestionsDone();
            Levels = new List<ILevelBarElement>();
            
            TopicID = model.TopicID;
            var topicModel = topicsRepository.GetByID(model.TopicID);
            TopicName = Settings.TopicNamesExtensions.Contains(topicModel.TopicName) ? topicModel.TopicName : topicModel.TopicName.ToUpper();
            IsHaveQuestions = questionsRepository.GetByMaterial(MaterialID).Any();

            var levelTypes = levelTypesRepository.GetAll();
            foreach (var levelType in levelTypes)
            {
                var materialModel = materialsRepository.GetForLevelAndTopic(levelType.LevelTypeID, TopicID);
                Levels.Add(new LevelViewModel(materialModel, levelType) {IsSelected = levelType.LevelTypeID == model.LevelTypeID});
            }
            TopicIconPath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topicModel.Image);
            AreaID = topicModel.AreaID;
            AreaName = areasRepository.GetNameByID(topicModel.AreaID);
            LevelName = Levels.First(x => x.IsSelected).Name;
            WriteHtmlFile();

            SetQuizButtonText();

            OnPropertyChanged(nameof(Levels));
        }

        public int GetMaterial(int topicId, int levelId)
        {
            var material = materialsRepository.GetForLevelAndTopic(levelId, topicId);
            LevelName = Levels.First(x => x.IsSelected).Name;
            return material.MaterialID;
        }

        public void SetQuizButtonText()
        {
            try
            {
                var oldScore = doneMaterialsRepository.GetEntriesForMaterial(MaterialID).FirstOrDefault(m => m.Adge == 0);
                TakeQuizButtonText = (oldScore == null ? StringResource.TakeQuizString : $"{StringResource.RetakeQuizString} {(oldScore.Score * 10)}%").ToUpper();
                OnPropertyChanged(nameof(TakeQuizButtonText));
            }
            catch (Exception ex)
            {
                logger.Log($"MaterialViewModel SetQuizButtonText error: {ex.Message}");
            }
        }

        private bool IsAllQuestionsDone()
        {
            var questions = questionsRepository.GetByMaterial(MaterialID);
            return questions.TrueForAll(q => doneQuestionsRepository.IsQuestionDone(q.QuestionID));

        }

        public void PropertiesChanged()
        {
            OnPropertyChanged(nameof(TakeQuizButtonText));
        }

        private void WriteHtmlFile()
        {
            FilePath = FileHelper.WriteFile(Text, FileTypes.Material);
            OnPropertyChanged(nameof(FilePath));
        }
    }
}

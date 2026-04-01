using System.Collections.Generic;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IQuestionsViewModel : IBasePurchasableViewModel
    {
        List<IQuestionViewModel> Questions { get; set; }

        int CurrentQuestionIndex { set; get; }

        IQuestionViewModel CurrentQuestion { get; }
        string CurrentFilePath { get; }

        void SetMaterialDone();

        void GetData();

        void SetCurrentQuestion();

        void ToNextQuestion();

        void ToQuestion(int questionIndex);

        int TopicID { get; set; }

        string TopicName { get; set; }
        string AreaName { get; set; } 
        string LevelName { get; set; } 

        string TopicIconPath { get; set; }

        int MaterialID { get; set; }

        IWordViewModel GetGlossryTerm(string reference);

        int NumberOfPages { get; }

        int CurrentPageNumber { get; }

        string PagerText { get; }
    }
}

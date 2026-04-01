using System.Collections.Generic;
using System.ComponentModel;
using BioBrain.ViewModels.Implementation;

namespace BioBrain.ViewModels.Interfaces
{
    public enum QuestionTypes
    {
        MultipleChoice = 0,
        FreeText = 1,
        DropDown = 2,
        CompleteSentence = 3,
        TrueFalse = 4,
        OrderList = 5,
        Swipe = 6,
    }

    public interface IQuestionViewModel : INotifyPropertyChanged
    {
        string QuestionText { get; }

        string FilePath { get; set; }

        QuestionTypes QuestionType { get; }

        List<IBaseAnswerViewModel> Answers { get; set; }

        int QuestionID { get; }

        string Hint { get; }

        bool IsHintEnabled { get; }

        int QuestionOrder { get; }

        AnswerResultViewModel CheckAnswer(WebViewResponseModel answer, bool isWriteFile = true);

        IBaseAnswerViewModel GetCorrectAnswer();

        bool IsSecondTry { get; set; }
    }
}

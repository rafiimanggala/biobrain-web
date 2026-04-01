using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IQuestionReviewViewModel
    {
        string FilePath { get; }

        string TopicName { get; }

        bool IsCorrect { get; }

        Color HexagonColor { get; }

        string QuestionHeader { get; }

        string TopicIconPath { get; set; }

        IWordViewModel GetGlossryTerm(string term);
    }
}
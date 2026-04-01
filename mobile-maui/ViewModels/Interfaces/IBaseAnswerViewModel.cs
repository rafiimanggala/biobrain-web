namespace BioBrain.ViewModels.Interfaces
{
    public interface IBaseAnswerViewModel
    {
        int AnswerID { get; }

        string Text { get; }

        bool IsCorrect { get; }

        bool CaseSensitive { get; }

        string FeedBack { get; }

        int Score { get; }

        int Order { get; }
    }
}
namespace DAL.Models.Interfaces
{
    public interface IAnswerModel
    {
        int AnswerID { get; set; }

        int QuestionID { get; set; }

        int AnswerOrder { get; set; }

        string Text { get; set; }

        bool IsCorrect { get; set; }

        bool CaseSensitive { get; set; }

        string Feedback { get; set; }

        int Score { get; set; }
    }
}
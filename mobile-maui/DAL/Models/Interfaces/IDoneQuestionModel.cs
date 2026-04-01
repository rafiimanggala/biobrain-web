namespace DAL.Models.Interfaces
{
    public interface IDoneQuestionModel
    {
        int DoneQuestionID { get; set; }

        int QuestionID { get; set; }

        int Score { get; set; }

        bool IsCorrect { get; set; }
    }
}
namespace DAL.Models.Interfaces
{
    public interface IQuestionModel
    {
        int QuestionID { get; set; }

        int MaterialID { get; set; }

        int QuestionType { get; set; }

        int QuestionOrder { get; set; }

        string Text { get; set; }

        string Hint { get; set; }
    }
}
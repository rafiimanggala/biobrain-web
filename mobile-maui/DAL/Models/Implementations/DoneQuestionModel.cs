using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("DoneQuestions")]
    public class DoneQuestionModel : IDoneQuestionModel
    {
        [PrimaryKey, AutoIncrement]
        public int DoneQuestionID { get; set; }

        public int QuestionID { get; set; }

        public int Score { get; set; }

        public bool IsCorrect { get; set; }
    }
}
using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Answers")]
    public class AnswerModel : IAnswerModel
    {
        [PrimaryKey]
        public int AnswerID { get; set; }

        public int QuestionID { get; set; }

        public int AnswerOrder { get; set; }

        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public bool CaseSensitive { get; set; }

        public string Feedback { get; set; }

        public int Score { get; set; }
    }
}
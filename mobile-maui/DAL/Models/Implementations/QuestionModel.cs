using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Questions")]
    public class QuestionModel : IQuestionModel
    {
        [PrimaryKey]
        public int QuestionID { get; set; }

        public int MaterialID { get; set; }

        public int QuestionType { get; set; }

        public int QuestionOrder { get; set; }

        public string Text { get; set; }

        public string Hint { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.WebAppEntities
{
    [Table("Answers")]
    public class AnswerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnswerId { get; set; }

        public Guid QuestionId { get; set; }
        public Guid CourseId { get; set; }

        public int AnswerOrder { get; set; }

        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public bool CaseSensitive { get; set; }

        public int Score { get; set; }

        public int Response { get; set; }
    }
}
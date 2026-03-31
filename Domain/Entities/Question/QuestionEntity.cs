using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Quiz;
using DataAccessLayer.WebAppEntities;

namespace Biobrain.Domain.Entities.Question
{
    [Table("Questions")]
    public class QuestionEntity: IDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuestionId { get; set; }

        public Guid CourseId { get; set; }

        public long QuestionTypeCode { get; set; }
        public QuestionTypeEntity QuestionType { get; set; }

        public string Header { get; set; }

        public string Text { get; set; }

        public string Hint { get; set; }

        public string FeedBack { get; set; }
        public List<AnswerEntity> Answers { get; set; }
        public List<QuizQuestionEntity> QuizQuestions { get; set; }
        public List<QuizExcludedQuestionEntity> QuizExcludedQuestions { get; set; }

        public ICollection<QuizResultQuestionEntity> QuizResultQuestions { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
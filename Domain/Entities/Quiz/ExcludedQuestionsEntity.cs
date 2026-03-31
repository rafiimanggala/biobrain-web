using System;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.SchoolClass;

namespace Biobrain.Domain.Entities.Quiz
{
    public class ExcludedQuestionEntity
    {
        public Guid ExcludedQuestionId { get; set; }
        
        public Guid SchoolClassId { get; set; }
        public SchoolClassEntity SchoolClass { get; set; }

        public Guid QuizId { get; set; }
        public QuizEntity Quiz { get; set; }

        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
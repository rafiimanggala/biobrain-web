using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Question;


namespace Biobrain.Domain.Entities.Quiz
{
    public sealed class QuizResultQuestionEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid QuizResultId { get; set; }
        public QuizResultEntity QuizResult { get; set; }

        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }

        public string Value { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

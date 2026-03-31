using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Question;


namespace Biobrain.Domain.Entities.Quiz
{
    public sealed class QuizEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid QuizId { get; set; }

        public Guid? AutoMapQuizId { get; set; }
        public QuizEntity? AutoMapQuiz { get; set; }

        public Guid ContentTreeId { get; set; }
        public ContentTreeEntity ContentTreeNode { get; set; }

        public QuizType Type { get; set; } = QuizType.Standard;
        public int? QuestionCount { get; set; }
        public string Name { get; set; }
        public Guid? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<QuizAssignmentEntity> Assignments { get; set; }
        public ICollection<QuizQuestionEntity> QuizQuestions { get; set; }
        public ICollection<QuizExcludedQuestionEntity> QuizExcludedQuestions { get; set; }
    }
}

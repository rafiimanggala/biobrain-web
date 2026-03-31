using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;


namespace Biobrain.Domain.Entities.Quiz
{
    public sealed class QuizResultEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid QuizStudentAssignmentId { get; set; }
        public QuizStudentAssignmentEntity QuizStudentAssignment { get; set; }

        public double Score { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime StaredAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<QuizResultQuestionEntity> Questions { get; set; }
    }
}

using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;


namespace Biobrain.Domain.Entities.Quiz
{
    public sealed class QuizStudentAssignmentEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid QuizStudentAssignmentId { get; set; }

        public Guid QuizAssignmentId { get; set; }
        public QuizAssignmentEntity QuizAssignment { get; set; }

        public Guid AssignedToUserId { get; set; }
        public UserEntity AssignedTo { get; set; }

        public QuizResultEntity Result { get; set; }

        public int AttemptNumber { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime? DueAtUtc { get; set; }
        public DateTime? DueAtLocal { get; set; }

        public DateTime? AssignedAtUtc { get; set; }
        public DateTime? AssignedAtLocal { get; set; }
    }
}

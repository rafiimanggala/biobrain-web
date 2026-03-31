using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;


namespace Biobrain.Domain.Entities.MaterialAssignments
{
    public sealed class LearningMaterialUserAssignmentEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid LearningMaterialUserAssignmentId { get; set; }

        public Guid LearningMaterialAssignmentId { get; set; }
        public LearningMaterialAssignmentEntity LearningMaterialAssignment { get; set; }

        public Guid AssignedToUserId { get; set; }
        public UserEntity AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public DateTime DueAtUtc { get; set; }
        public DateTime DueAtLocal { get; set; }

        public DateTime AssignedAtUtc { get; set; }
        public DateTime AssignedAtLocal { get; set; }
    }
}

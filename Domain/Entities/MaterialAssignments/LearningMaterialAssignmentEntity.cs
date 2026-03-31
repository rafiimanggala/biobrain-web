using System;
using System.Collections.Generic;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;


namespace Biobrain.Domain.Entities.MaterialAssignments
{
    public sealed class LearningMaterialAssignmentEntity
    {
        public Guid LearningMaterialAssignmentId { get; set; }

        public Guid ContentTreeNodeId { get; set; }
        public ContentTreeEntity ContentTreeNode { get; set; }

        public Guid? AssignedByUserId { get; set; }
        public UserEntity AssignedBy { get; set; }

        public Guid? SchoolClassId { get; set; }
        public SchoolClassEntity SchoolClass { get; set; }

        public ICollection<LearningMaterialUserAssignmentEntity> UserAssignments { get; set; }

        public DateTime DueAtUtc { get; set; }
        public DateTime DueAtLocal { get; set; }

        public DateTime AssignedAtUtc { get; set; }
        public DateTime AssignedAtLocal { get; set; }

        public bool IsForWholeClass { get; set; }
    }
}

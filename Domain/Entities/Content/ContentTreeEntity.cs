using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.MaterialAssignments;
using DataAccessLayer.WebAppEntities;

namespace Biobrain.Domain.Entities.Content
{
    public class ContentTreeEntity : IDeletedEntity
    {
        public Guid NodeId { get; set; }
        public Guid CourseId { get; set; }
        public Guid ContentTreeMetaId { get; set; }
        public ContentTreeMetaEntity ContentTreeMeta { get; set; }
        public CourseEntity Course { get; set; }

        public Guid? ParentId { get; set; }
        public ContentTreeEntity ParentContentTree { get; set; }

        public string Name { get; set; }
        public long Order { get; set; }
        public bool IsAvailableInDemo { get; set; }

        public Guid? IconId { get; set; }
        public IconEntity Icon { get; set; }

        public ICollection<LearningMaterialAssignmentEntity> Assignments { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
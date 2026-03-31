using System;
using System.Collections.Generic;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.Teacher;


namespace Biobrain.Domain.Entities.Quiz
{
    public sealed class QuizAssignmentEntity
    {
        public Guid QuizAssignmentId { get; set; }

        public Guid QuizId { get; set; }
        public QuizEntity Quiz { get; set; }

        public Guid? AssignedByTeacherId { get; set; }
        public TeacherEntity AssignedBy { get; set; }

        public Guid? SchoolClassId { get; set; }
        public SchoolClassEntity SchoolClass { get; set; }

        public DateTime? DueAtUtc { get; set; }
        public DateTime? DueAtLocal { get; set; }

        public DateTime? AssignedAtUtc { get; set; }
        public DateTime? AssignedAtLocal { get; set; }

        public bool HintsEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        public bool IncludeLearningMaterial { get; set; } = true;

        public ICollection<QuizStudentAssignmentEntity> QuizStudentAssignments { get; set; }
    }
}

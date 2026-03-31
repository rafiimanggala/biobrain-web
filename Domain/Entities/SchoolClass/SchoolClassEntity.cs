using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.MaterialAssignments;
using Biobrain.Domain.Entities.Quiz;
using Biobrain.Domain.Entities.School;

namespace Biobrain.Domain.Entities.SchoolClass
{
    public class SchoolClassEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid SchoolClassId { get; set; }
        public Guid SchoolId { get; set; }
        public int Year { get; set; }  // 9, 10, 11, 12 for now
        public string Name { get; set; }
        public Guid CourseId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string AutoJoinClassCode { get; set; }
        
        public SchoolEntity School { get; set; }
        public CourseEntity Course { get; set; }
        public ICollection<SchoolClassStudentEntity> Students { get; set; }
        public ICollection<SchoolClassTeacherEntity> Teachers { get; set; }
        public ICollection<QuizAssignmentEntity> AssignedQuizzes { get; set; }
        public ICollection<LearningMaterialAssignmentEntity> AssignedMaterials { get; set; }
    }
}

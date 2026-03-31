using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Quiz;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Teacher
{
    public class TeacherEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZoneId { get; set; }
        public ICollection<SchoolTeacherEntity> Schools { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserEntity User { get; set; }

        public ICollection<QuizAssignmentEntity> AssignedByTeacherQuizzes { get; set; }
        public ICollection<SchoolClassTeacherEntity> Classes { get; set; }

        public string GetFullName() => $"{FirstName} {LastName}";
    }
}
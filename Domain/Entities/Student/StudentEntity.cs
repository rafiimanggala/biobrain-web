using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Student
{
    public class StudentEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZoneId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int? CurriculumCode { get; set; }
        public int? Year { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserEntity User { get; set; }
        public CurriculumEntity Curriculum { get; set; }

        public ICollection<SchoolStudentEntity> Schools { get; set; }
        public ICollection<SchoolClassStudentEntity> SchoolClasses { get; set; }

        public string GetFullName() => $"{FirstName} {LastName}";
    }
}
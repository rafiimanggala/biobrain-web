using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Student;

namespace Biobrain.Domain.Entities.SchoolClass
{
    public class SchoolClassStudentEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid SchoolClassId { get; set; }
        public Guid StudentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SchoolClassEntity SchoolClass { get; set; }
        public StudentEntity Student { get; set; }
    }
}
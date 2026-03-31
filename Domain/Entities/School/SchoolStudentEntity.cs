using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Student;

namespace Biobrain.Domain.Entities.School
{
    public class SchoolStudentEntity : ICreatedEntity
    {
	    public Guid SchoolId { get; set; }
        public Guid StudentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public StudentEntity Student { get; set; }
        public SchoolEntity School { get; set; }
    }
}

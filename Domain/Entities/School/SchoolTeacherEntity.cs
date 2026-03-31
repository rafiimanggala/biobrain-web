using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Teacher;

namespace Biobrain.Domain.Entities.School
{
    public class SchoolTeacherEntity : ICreatedEntity
    {
        public Guid SchoolId { get; set; }
        public Guid TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }

        public TeacherEntity Teacher { get; set; }
        public SchoolEntity School { get; set; }
    }
}

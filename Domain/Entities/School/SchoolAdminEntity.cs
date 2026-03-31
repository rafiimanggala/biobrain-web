using System;
using Biobrain.Domain.Entities.Teacher;

namespace Biobrain.Domain.Entities.School
{
    public class SchoolAdminEntity
    {
        public Guid SchoolId { get; set; }
        public Guid TeacherId { get; set; }

        public SchoolEntity School { get; set; }
        public TeacherEntity Teacher { get; set; }
    }
}
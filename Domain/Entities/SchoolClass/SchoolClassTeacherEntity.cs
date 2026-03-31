using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Teacher;

namespace Biobrain.Domain.Entities.SchoolClass
{
    public class SchoolClassTeacherEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid TeacherId { get; set; }
        public Guid SchoolClassId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TeacherEntity Teacher { get; set; }
        public SchoolClassEntity SchoolClass { get; set; }
    }
}

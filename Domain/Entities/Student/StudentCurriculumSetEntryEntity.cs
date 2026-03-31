using System;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.Student
{
    public class StudentCurriculumSetEntryEntity
    {
        public Guid StudentCurriculumSetId { get; set; }
        public StudentCurriculumSetEntity Set { get; set; }

        public Guid CourseId { get; set; }
        public CourseEntity Course { get; set; }

        public string DisplayName { get; set; }
    }
}
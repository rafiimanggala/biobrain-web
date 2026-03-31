using System;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.School
{
    public class SchoolCourseEntity
    {
        public Guid SchoolCourseId { get; set; }

        public Guid SchoolId { get; set; }
        public SchoolEntity School { get; set; }

        public Guid CourseId { get; set; }
        public CourseEntity Course { get; set; }
    }
}
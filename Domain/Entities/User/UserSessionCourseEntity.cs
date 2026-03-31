using System;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.User
{
    public class UserSessionCourseEntity
    {
        public Guid UserSessionCourseEntityId { get; set; }

        public Guid UserSessionId { get; set; }
        public UserSessionEntity UserSession { get; set; }

        public Guid? CourseId { get; set; }
        public CourseEntity Course { get; set; }
    }
}
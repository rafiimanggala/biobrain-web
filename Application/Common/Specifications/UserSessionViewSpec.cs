using System;
using System.Linq;
using Biobrain.Domain.Entities.User;


namespace Biobrain.Application.Specifications
{
    public static class UserSessionViewSpec
    {
        public static Spec<UserSessionEntity> ForCourse(Guid courseId) => new(_ => _.Courses.Any(_ => _.CourseId == courseId));
        public static Spec<UserSessionEntity> ForSchool(Guid schoolId) => new(_ => _.Schools.Any(_ => _.SchoolId == schoolId));
    }
}

using System;
using Biobrain.Domain.Entities.User;


namespace Biobrain.Application.Specifications
{
    public static class UserPageViewSpec
    {
        public static Spec<UserPageViewEntity> ForCourse(Guid courseId) => new(_ => _.CourseId == courseId);
        public static Spec<UserPageViewEntity> ForSchool(Guid schoolId) => new(_ => _.SchoolId == schoolId);
    }
}

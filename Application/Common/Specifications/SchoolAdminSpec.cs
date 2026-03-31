using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.School;

namespace Biobrain.Application.Specifications
{
    public static class SchoolAdminSpec
    {
        public static Spec<SchoolAdminEntity> ForSchool(Guid schoolId) => new(_ => _.SchoolId == schoolId);
        public static Spec<SchoolAdminEntity> ForTeacher(Guid teacherId) => new(_ => _.TeacherId == teacherId);
        public static Spec<SchoolAdminEntity> ForTeachers(IEnumerable<Guid> teacherIds) => new(_ => teacherIds.Contains(_.TeacherId));
    }
}
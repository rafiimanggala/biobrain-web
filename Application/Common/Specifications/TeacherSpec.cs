using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Teacher;

namespace Biobrain.Application.Specifications
{
    public static class TeacherSpec
    {
        public static Spec<TeacherEntity> ForSchool(Guid schoolId) => new(_ => _.Schools.Any(x => x.SchoolId == schoolId));

        public static Spec<TeacherEntity> ById(Guid teacherId) => new(_ => _.TeacherId == teacherId);

        public static Spec<TeacherEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.TeacherId));

        public static Spec<TeacherEntity> ForClass(Guid schoolClassId) => new(_ => _.Classes.Any(c => c.SchoolClassId == schoolClassId));

        public static Spec<TeacherEntity> WithSimilarName(string searchText)
        {
            return new(_ => searchText == null || _.FirstName.Contains(searchText) || _.LastName.Contains(searchText));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.SchoolClass;

namespace Biobrain.Application.Specifications
{
    public static class SchoolClassSpec
    {
        public static Spec<SchoolClassEntity> ForCourse(Guid courseId) => new(_ => _.CourseId == courseId);
        public static Spec<SchoolClassEntity> ForSchool(Guid schoolId) => new(_ => _.SchoolId == schoolId);
        public static Spec<SchoolClassEntity> ForYear(int year) => new(_ => _.Year == year);
        public static Spec<SchoolClassEntity> WithName(string name) => new(_ => _.Name == name);
        public static Spec<SchoolClassEntity> ById(Guid id) => new(_ => _.SchoolClassId == id);
        public static Spec<SchoolClassEntity> OtherSchoolClasses(Guid id) => new(_ => _.SchoolClassId != id);
        public static Spec<SchoolClassEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.SchoolClassId));
        public static Spec<SchoolClassEntity> WithQuizAssignments(IEnumerable<Guid> quizAssignmentIds) 
            => new(_ => _.AssignedQuizzes.Any(x => quizAssignmentIds.Any(z => z == x.QuizAssignmentId)));

        public static Spec<SchoolClassEntity> ByClassCode(string classCode)
        {
            var classCodeParam = classCode.ToUpper();
            return new(_ => _.AutoJoinClassCode == classCodeParam);
        }
    }
}

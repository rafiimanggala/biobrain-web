using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Student;

namespace Biobrain.Application.Common.Specifications
{
    public static class StudentSpec
    {
	    public static Spec<StudentEntity> ForSchool(Guid schoolId) => new(_ => _.Schools.Any(x => x.SchoolId == schoolId));

        public static Spec<StudentEntity> ForDates(DateTime from, DateTime to) => new(_ => _.CreatedAt >= from && _.CreatedAt <= to);

        public static Spec<StudentEntity> ById(Guid id) => new(_ => _.StudentId == id);

        public static Spec<StudentEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.StudentId));

        public static Spec<StudentEntity> ForClass(Guid schoolClassId) => new(_ => _.SchoolClasses.Any(c => c.SchoolClassId == schoolClassId));

        public static Spec<StudentEntity> ForQuizAssignment(Guid quizAssignmentId)
            => new(_ => _.User.AssignedQuizzes.Any(x => x.QuizAssignmentId == quizAssignmentId));
    }
}
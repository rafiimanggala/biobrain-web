using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Specifications
{
    public static class QuizSpec
    {
        public static Spec<QuizEntity> ForCourse(Guid courseId) => new(_ => _.ContentTreeNode.CourseId == courseId);

        public static Spec<QuizEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.QuizId));
        public static Spec<QuizEntity> ById(Guid id) => new(_ => id == _.QuizId);
    }
}

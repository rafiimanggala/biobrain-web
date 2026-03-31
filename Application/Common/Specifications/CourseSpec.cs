using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Course;


namespace Biobrain.Application.Specifications
{
    public static class CourseSpec
    {
        public static Spec<CourseEntity> ById(Guid courseId) => new(_ =>_.CourseId == courseId);
        public static Spec<CourseEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.CourseId));
    }
}

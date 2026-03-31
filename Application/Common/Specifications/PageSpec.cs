using System;
using Biobrain.Domain.Entities.Material;


namespace Biobrain.Application.Specifications
{
    public static class PageSpec
    {
        public static Spec<PageEntity> ForCourse(Guid courseId) => new(_ => _.ContentTreeNode.CourseId == courseId);
    }
}

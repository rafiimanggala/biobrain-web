using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Content;


namespace Biobrain.Application.Specifications
{
    public static class ContentTreeSpec
    {
        public static Spec<ContentTreeEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.NodeId));
        public static Spec<ContentTreeEntity> ById(Guid id) => new(_ => id == _.NodeId);
        public static Spec<ContentTreeEntity> ForCourse(Guid courseId) => new(_ => _.CourseId == courseId);
    }
}

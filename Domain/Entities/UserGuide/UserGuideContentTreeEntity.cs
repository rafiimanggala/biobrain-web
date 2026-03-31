using System;

namespace Biobrain.Domain.Entities.UserGuide
{
    public class UserGuideContentTreeEntity
    {
        public Guid NodeId { get; set; }

        public Guid? ParentId { get; set; }
        public UserGuideContentTreeEntity Parent { get; set; }

        public string Name { get; set; }
        public long Order { get; set; }
        public bool IsAvailableForStudent { get; set; }

        public UserGuideArticleEntity Article { get; set; }
    }
}
using System;

namespace Biobrain.Domain.Entities.UserGuide
{
    public class UserGuideArticleEntity
    {
        public Guid UserGuideArticleId { get; set; }

        public Guid UserGuideContentTreeId { get; set; }
        public UserGuideContentTreeEntity UserGuideContentTreeNode { get; set; }

        public string HtmlText { get; set; }
        public string VideoLink { get; set; }
    }
}
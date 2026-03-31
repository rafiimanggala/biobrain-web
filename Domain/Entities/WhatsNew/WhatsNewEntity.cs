using System;

namespace Biobrain.Domain.Entities.WhatsNew
{
    public class WhatsNewEntity
    {
        public Guid WhatsNewId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Version { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

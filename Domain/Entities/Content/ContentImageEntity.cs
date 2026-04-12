using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Content
{
    public class ContentImageEntity : ICreatedEntity
    {
        public Guid ImageId { get; set; }
        public string Code { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Description { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long FileSize { get; set; }
        public Guid? UploadedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

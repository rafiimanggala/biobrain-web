using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Notifications
{
    public class EmailMessageEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid EmailMessageId { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }

        public int Attempts { get; set; }
        public string? FailReason { get; set; }
        public bool IsDisabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

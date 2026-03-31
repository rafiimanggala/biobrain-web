using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.AccessCodes
{
    public class AccessCodeMilestoneEntity : ICreatedEntity
    {
        public Guid AccessCodeId { get; set; }

        public Guid BatchId { get; set; }
        public AccessCodeBatchEntity Batch { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public string Code { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}
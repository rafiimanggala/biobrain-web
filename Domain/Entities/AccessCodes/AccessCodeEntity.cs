using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.AccessCodes
{
    public class AccessCodeEntity: ICreatedEntity
    {
        public Guid AccessCodeId { get; set; }

        public Guid BatchId { get; set; }
        public AccessCodeBatchEntity Batch { get; set; }

        public string Code { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}
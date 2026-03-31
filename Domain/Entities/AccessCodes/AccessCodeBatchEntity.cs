using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.AccessCodes
{
    public class AccessCodeBatchEntity : ICreatedEntity
    {
        public Guid AccessCodeBatchId { get; set; }

        public string Note { get; set; }
        public int NumberOfCodes { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<AccessCodeEntity> AccessCodes { get; set; }
        public IEnumerable<AccessCodeMilestoneEntity> UsedAccessCodes { get; set; }
        public IEnumerable<AccessCodeBatchCourseEntity> Courses { get; set; }
    }
}
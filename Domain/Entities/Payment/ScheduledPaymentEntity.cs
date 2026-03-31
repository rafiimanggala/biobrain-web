using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Payment
{
    public class ScheduledPaymentEntity: ICreatedEntity, IUpdatedEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Guid ScheduledPaymentId { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public PaymentPeriods Period { get; set; }
        public ScheduledPaymentStatus Status { get; set; }
        public ScheduledPaymentType Type { get; set; }

        /// <summary>
        /// Pay mark that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </summary>
        public int PayDate { get; set; }
        
        /// <summary>
        /// Pay mark that has format mmddhh in utc when leap year
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </summary>
        public int LeapPayDate { get; set; }

        public string Description { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public List<ScheduledPaymentCourseEntity> ScheduledPaymentCourses { get; set; }
    }
}
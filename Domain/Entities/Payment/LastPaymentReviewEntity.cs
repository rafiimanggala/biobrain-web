using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Payment
{
    public class LastPaymentReviewEntity : ICreatedEntity, IUpdatedEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid LastPaymentReviewId { get; set; }

        /// <summary>
        /// Pay mark that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </summary>
        public int PayDate { get; set; }
    }
}
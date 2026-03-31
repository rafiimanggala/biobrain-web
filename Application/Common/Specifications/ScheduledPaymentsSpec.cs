using Biobrain.Domain.Entities.Payment;


namespace Biobrain.Application.Specifications
{
    public static class ScheduledPaymentSpec
    {
        public static Spec<ScheduledPaymentEntity> ForLeapDates(LastPaymentReviewEntity from,
	        LastPaymentReviewEntity to) => new(x => x.LeapPayDate > from.PayDate && x.LeapPayDate <= to.PayDate);
    }
}

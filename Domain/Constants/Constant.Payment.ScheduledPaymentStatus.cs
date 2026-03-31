namespace Biobrain.Domain.Constants
{
    public enum ScheduledPaymentStatus
    {
        Created = 0,
        Success = 1,
        PaymentFailed = 2,
        StoppedByUser = 3,
        Inactive = 4
    }
}
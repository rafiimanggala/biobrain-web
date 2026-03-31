namespace Biobrain.Infrastructure.Payments.PinPayments
{
    public static class PinEvents
    {
        public const string ChargeAuthorised = "charge.authorised";
        public const string ChargeCaptured = "charge.captured";
        public const string ChargeFailed = "charge.failed";
        public const string ChargeRefunded = "charge.refunded";
        public const string CustomerCreated = "customer.created";
        public const string CustomerUpdated = "customer.updated";
        public const string CustomerDeleted = "customer.deleted";
        public const string RecipientCreated = "recipient.created";
        public const string RecipientUpdated = "recipient.updated";
        public const string RecipientDeleted = "recipient.deleted";
        public const string RefundCreated = "refund.created";
        public const string TransferCreated = "transfer.created";
    }
}

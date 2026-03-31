using Biobrain.Domain.Constants;
using Biobrain.Infrastructure.Payments.Services.Payment;

namespace Biobrain.Infrastructure.Payments.Models
{
	public class PaymentMethodModel
	{
		public IPaymentProvider PaymentProvider { get; init; }
		public PaymentMethods PaymentMethod { get; init; }
	}
}
using System.Threading.Tasks;
using Biobrain.Application.Payments.Models;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Application.Interfaces.Payments
{
	public interface IPaymentService
	{
		Task<PaymentEntity> Pay(ScheduledPaymentEntity scheduledPayment, UserPaymentDetailsEntity userPayment, PromoCodeEntity promoCode = null);
		Task<string> GetCardToken(CardModel model, PaymentMethods paymentMethod);
		Task<string> PostCustomerAsync(string email, string cardToken, PaymentMethods paymentMethod);
		Task PutCustomerAsync(string customerToken, string cardToken, string email, PaymentMethods paymentMethod);
	}
}
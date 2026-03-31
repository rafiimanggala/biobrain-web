using System;
using System.Threading.Tasks;
using Biobrain.Application.Payments.Models;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Infrastructure.Payments.Services.Payment
{
	public interface IPaymentProvider
	{
		Task<PaymentEntity> Pay(double amount, string currency, UserPaymentDetailsEntity userPayment, PaymentEntity payment, Guid scheduledPaymentId);
		Task<string> GetCardToken(CardModel model);
		Task<string> PostCustomerAsync(string email, string cardToken);
		Task PutCustomerAsync(string customerToken, string cardToken, string email);
	}
}
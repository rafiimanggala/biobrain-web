using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Payments.Models;
using Biobrain.Application.Values.Options;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Infrastructure.Payments.ErrorHandling;
using Biobrain.Infrastructure.Payments.PinPayments.Cards;
using Biobrain.Infrastructure.Payments.PinPayments.Charges;
using Biobrain.Infrastructure.Payments.PinPayments.Customers;
using Biobrain.Infrastructure.Payments.PinPayments.Models;
using Biobrain.Infrastructure.Payments.PinPayments.Transfers;
using BiobrainWebAPI.Values;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PinPayments;

namespace Biobrain.Infrastructure.Payments.Services.Payment
{
	public class PinPaymentsProvider : IPaymentProvider
	{
		private readonly ILogger _logger;
		private readonly IDb _db;
		private readonly IConfiguration _configuration;

		private readonly IDynamicPinChargeService _pinChargeService;
		private readonly IDynamicPinTransferService _pinTransferService;
		private readonly IDynamicPinCardService _pinCardService;
		private readonly IDynamicPinCustomerService _customerService;

		public PinPaymentsProvider(ILogger<PinPaymentsProvider> logger, IDb db, IConfiguration configuration,
			IDynamicPinChargeService pinChargeService, IDynamicPinTransferService pinTransferService, IDynamicPinCardService pinCardService,
			IDynamicPinCustomerService customerService)
		{
			_logger = logger;
			_db = db;
			_configuration = configuration;
			_pinChargeService = pinChargeService;
			_pinTransferService = pinTransferService;
            _pinCardService = pinCardService;
            _customerService = customerService;
		}

		/// <summary>
		/// Initiate scheduled payment
		/// </summary>
		/// <param name="currency"></param>
		/// <param name="userPayment"></param>
		/// <param name="payment"></param>
		/// <param name="amount"></param>
		/// <param name="scheduledPaymentId"></param>
        /// <returns></returns>
		public async Task<PaymentEntity> Pay(double amount, string currency, UserPaymentDetailsEntity userPayment, PaymentEntity payment, Guid scheduledPaymentId)
        {
            var pinOptions = _configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
            if (pinOptions == null)
	            throw new PaymentException("Missing configuration for PinPayments");

            try
            {
                Charge charge = null;

				if (amount > 1)
                {
                    charge = await CreateCharge(amount, currency, userPayment, pinOptions.SecretKey);
                    _logger.LogInformation(
                        $"Charge created: {JsonConvert.SerializeObject(charge)}, ApiKey: {pinOptions.SecretKey}");
                }

                payment = UpdatePayment(payment, charge);
            }
            catch (PinException e)
            {
                _logger.LogError(e, $"Charge exception by PinPayment. ScheduledPayment: {scheduledPaymentId}");
                payment = await UpdatePayment(payment, e, PaymentStatusEnum.ChargeFailed);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Charge exception. ScheduledPayment: {scheduledPaymentId}");
                throw;
            }
			
            return payment;
        }

		public async Task<string> GetCardToken(CardModel model)
		{
			var pinOptions = _configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
			if (pinOptions == null)
				throw new PaymentException("Missing configuration for PinPayments");

            var card = new Card
			{
				Number = model.CardNumber,
				ExpiryMonth = model.ExpiryMonth,
				ExpiryYear = model.ExpiryYear,
				Cvc = model.Cvc,
				Name = model.CardholderName,
                AddressLine1 = model.AddressLine1,
                AddressCity = model.City,
                AddressCountry = model.Country
			};

			try
			{
				var addedCard = await _pinCardService.PostCardAsync(card, pinOptions.SecretKey);
				return addedCard.Token;
			}
			catch (PinException e)
			{
				_logger.LogError(e, $"Get card token exception");
				throw new PaymentException(e.Error.Description, e.Error?.Messages?.Select(_ => new ValidationFailure(_.Param, _.Message)));
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Get card token exception");
				throw;
			}
        }

		public async Task<string> PostCustomerAsync(string email, string cardToken)
		{
			var pinOptions = _configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
			if (pinOptions == null)
				throw new PaymentException("Missing configuration for PinPayments");
            try
			{
				var customer = await _customerService.PostCustomerAsync(new Customer
				{
					Email = email,
					CardToken = cardToken,

				}, pinOptions.SecretKey);
				return customer.Token;
			}
			catch (PinException e)
			{
				_logger.LogError(e, $"Post customer exception");
				throw new PaymentException(e.Error.Description, e.Error?.Messages?.Select(_ => new ValidationFailure(_.Param, _.Message)));
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Post customer exception");
				throw;
			}

		}

		public async Task PutCustomerAsync(string customerToken, string cardToken, string email)
		{
			var pinOptions = _configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
			if (pinOptions == null)
				throw new PaymentException("Missing configuration for PinPayments");
			try
			{
				var card = await _customerService.PostNewCardToCustomerAsync(customerToken, new CardTokenModel()
				{
					CardToken = cardToken,

				}, pinOptions.SecretKey);
				await _customerService.SetCardAsDefaultAsync(customerToken, card.Token, email, pinOptions.SecretKey);
				var cards = await _customerService.GetCustomerCardsAsync(customerToken, pinOptions.SecretKey);

				var cardsToDelete = cards.Where(x => x.Token != card.Token);
				foreach (var cardToDelete in cardsToDelete)
				{
					await _customerService.RemoveNotDefaultCustomerCardAsync(customerToken,
						cardToDelete.Token, pinOptions.SecretKey);
				}
			}
			catch (PinException e)
			{
				_logger.LogError(e, $"Put customer exception");
				throw new PaymentException(e.Error.Description, e.Error?.Messages?.Select(_ => new ValidationFailure(_.Param, _.Message)));
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Put customer exception");
				throw;
			}

		}

		private async Task<Charge> CreateCharge(double amount, string currency, UserPaymentDetailsEntity userPayment, string privateApiKey) => await _pinChargeService.PostChargeAsync(new Charge
                                                                                                                                                       {
                                                                                                                                                           Email = userPayment.User.Email,
                                                                                                                                                           Description = $"Payment for courses from {userPayment.User.GetFullName()}.",
                                                                                                                                                           Amount = (int)Math.Truncate(amount * 100),
                                                                                                                                                           IpAddress = userPayment.IpAddress,
                                                                                                                                                           Currency = currency,
                                                                                                                                                           CustomerToken = userPayment.PinPaymentCustomerRefId,
                                                                                                                                                           Capture = true
                                                                                                                                                       }, privateApiKey);

        private PaymentEntity UpdatePayment(PaymentEntity payment, Charge charge)
        {
            if (charge != null)
            {
                payment.ChargeRefId = charge.Token;
                if (charge.Success)
                {
                    payment.Status = PaymentStatusEnum.ChargeSuccess;
                    //_db.Update(payment).State = EntityState.Modified;
                }
                else
                {
                    payment.Status = PaymentStatusEnum.ChargeFailed;
                    payment.FailedPayload = JsonConvert.SerializeObject(charge);
                }
            }
            else
            {
                payment.ChargeRefId = payment.PaymentId.ToString();
                payment.Status = PaymentStatusEnum.ChargeSuccess;
			}

            //await _db.SaveChangesAsync();
			return payment;
		}

        private async Task<PaymentEntity> UpdatePayment(PaymentEntity payment, PinException exception, PaymentStatusEnum status)
        {
            payment.Status = status;
            payment.FailedPayload = exception.ToString();

            await _db.SaveChangesAsync();
            return payment;
        }
    }
}
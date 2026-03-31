using System;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Values.Options;
using Biobrain.Infrastructure.Payments.PinPayments;
using Biobrain.Infrastructure.Payments.PinPayments.Cards;
using Biobrain.Infrastructure.Payments.PinPayments.Charges;
using Biobrain.Infrastructure.Payments.PinPayments.Customers;
using Biobrain.Infrastructure.Payments.PinPayments.Recipients;
using Biobrain.Infrastructure.Payments.PinPayments.Transfers;
using Biobrain.Infrastructure.Payments.Services.Hosted;
using Biobrain.Infrastructure.Payments.Services.Payment;
using Biobrain.Infrastructure.Payments.Services.PaymentDate;
using Biobrain.Infrastructure.Payments.Services.PaymentHistory;
using Biobrain.Infrastructure.Payments.Services.ScheduledPayment;
using BiobrainWebAPI.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biobrain.Infrastructure.Payments
{
    public static class DependencyInjection
	{
		public static IServiceCollection AddPayments(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IPaymentDateService, PaymentDateService>();
			services.AddScoped<IScheduledPaymentService, ScheduledPaymentService>();
			services.AddScoped<PinPaymentsProvider, PinPaymentsProvider>();
			services.AddScoped<IPaymentHistoryService, PaymentHistoryService>();

			services.AddPinPayments(configuration);
			services.AddHostedService<PaymentHostedService>();

			return services;
		}


		private static IServiceCollection AddPinPayments(this IServiceCollection services, IConfiguration configuration)
		{
			var pinConfig = configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
			if (pinConfig == null) throw new Exception("PinPayments configuration missing");

			//PinPayment
			services.AddSingleton(x =>
				new PinPaymentsOptions("", pinConfig.BaseUrl));

			services.AddScoped<IDynamicPinChargeService, DynamicPinChargeService>();
			services.AddScoped<IDynamicPinCustomerService, DynamicPinCustomerService>();
			services.AddScoped<IDynamicPinRecipientService, DynamicPinRecipientService>();
			services.AddScoped<IDynamicPinTransferService, DynamicPinTransferService>();
			services.AddScoped<IDynamicPinCardService, DynamicPinCardService>();

			return services;
		}
	}
}

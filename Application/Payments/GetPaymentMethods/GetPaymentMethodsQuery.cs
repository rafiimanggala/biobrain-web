using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Values.Options;
using BiobrainWebAPI.Values;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.Payments.GetPaymentMethods
{
    [PublicAPI]
    public class GetPaymentMethodsQuery : ICommand<List<GetPaymentMethodsQuery.Result>>
    {
        [PublicAPI]
        public class Result
        {
            public string Name { get; init; }
            public string PublicKey { get; init; }
            public string ApiBaseUrl { get; init; }
        }

        internal class Handler(IDb db, IConfiguration configuration) : CommandHandlerBase<GetPaymentMethodsQuery, List<Result>>(db)
        {
	        private readonly IConfiguration _configuration = configuration;

            public override Task<List<Result>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
            {
                var methods = new List<Result>()
                {
	                //new() { Name = "PayPal (Coming Soon)" },
                };

                var pinPaymentsOptions = _configuration.GetSection(ConfigurationSections.PinPayments)?.Get<PinPaymentOptions>();
                if (pinPaymentsOptions != null)
	                methods.Add(new()
	                {
		                Name = "Credit and Debit Cards (Pin Payments)",
		                ApiBaseUrl = pinPaymentsOptions.BaseUrl,
		                PublicKey = pinPaymentsOptions.PublishableKey
	                });

                return Task.FromResult(methods);
            }
        }
    }
}
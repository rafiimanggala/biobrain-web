using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Payments.Models;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.Payments.GetCardToken
{
    [PublicAPI]
    public class GetCardTokenCommand : ICommand<GetCardTokenCommand.Result>
    {
	    public string CardNumber { get; set; }
	    public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvc { get; set; }
        public string CardholderName { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [PublicAPI]
        public class Result
        {
            public string CardToken { get; init; }
        }


        internal class PermissionCheck : PermissionCheckBase<GetCardTokenCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(GetCardTokenCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent()) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<GetCardTokenCommand, Result>
        {
	        private readonly IConfiguration _configuration;
	        private readonly IPaymentService _paymentService;
            public Handler(IDb db, IConfiguration configuration, IPaymentService paymentService) : base(db)
            {
	            _configuration = configuration;
	            _paymentService = paymentService;
            }

            public override async Task<Result> Handle(GetCardTokenCommand request, CancellationToken cancellationToken)
            {
	            var cardToken = await _paymentService.GetCardToken(new CardModel
	            {
		            Country = request.Country,
		            AddressLine1 = request.AddressLine1,
		            City = request.City,
		            CardNumber = request.CardNumber,
		            CardholderName = request.CardholderName,
		            Cvc = request.Cvc,
		            ExpiryMonth = request.ExpiryMonth,
		            ExpiryYear = request.ExpiryYear
	            }, PaymentMethods.PinPayments);

                return new Result{ CardToken = cardToken };
            }
        }
    }
}
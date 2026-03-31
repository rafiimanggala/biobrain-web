using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.UpdatePaymentDetails
{
    [PublicAPI]
    public class UpdatePaymentDetailsCommand : ICommand<UpdatePaymentDetailsCommand.Result>
    {
        public Guid UserId { get; set; }
        public string CardToken { get; init; }
        public string IpAddress { get; set; }

        [PublicAPI]
        public class Result
        {
            
        }




        internal class Validator : ValidatorBase<UpdatePaymentDetailsCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
	        }
        }


		internal class PermissionCheck : PermissionCheckBase<UpdatePaymentDetailsCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(UpdatePaymentDetailsCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent()) return false;
		        if (!user.IsAccountOwner(request.UserId)) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<UpdatePaymentDetailsCommand, Result>
        {
	        private readonly IPaymentService _paymentService;
            public Handler(IDb db, IPaymentService paymentService) : base(db) => _paymentService = paymentService;

            public override async Task<Result> Handle(UpdatePaymentDetailsCommand request, CancellationToken cancellationToken)
            {
	            var student = await Db.Users.Where(x => x.Id == request.UserId).Include(x => x.Student).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                
				await AddOrUpdateUserPayment(student, request.CardToken, request.IpAddress);

				return new Result();
            }


            private async Task AddOrUpdateUserPayment(UserEntity user, string cardToken, string ipAddress)
            {
	            var userPayment = await Db.UserPaymentDetails
		            .Where(x => x.UserId == user.Id && x.PaymentMethod == PaymentMethods.PinPayments)
		            .FirstOrDefaultAsync();

                if (userPayment == null)
                {
	                if (string.IsNullOrEmpty(cardToken))
		                throw new ValidationException("No payment details");

		            var customerToken =
			            await _paymentService.PostCustomerAsync(user.Email, cardToken, PaymentMethods.PinPayments);
		            Db.UserPaymentDetails.Add(new UserPaymentDetailsEntity
		            {
                        IpAddress = ipAddress,
                        PaymentMethod = PaymentMethods.PinPayments,
                        PinPaymentCustomerRefId = customerToken,
                        UserId = user.Id,
                        UserPaymentId = Guid.NewGuid()
                    });
	            }
                else
                {
	                userPayment.IpAddress = ipAddress;
	                await _paymentService.PutCustomerAsync(userPayment.PinPaymentCustomerRefId, cardToken, user.Email, PaymentMethods.PinPayments);
                }

                await Db.SaveChangesAsync();

            }
        }
    }
}
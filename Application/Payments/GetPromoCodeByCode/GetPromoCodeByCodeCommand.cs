using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.GetPromoCodeByCode
{
    [PublicAPI]
    public class GetPromoCodeByCodeCommand : ICommand<GetPromoCodeByCodeCommand.Result>
    {
	    public string PromoCode { get; set; }
        public PaymentPeriods PaymentPeriod { get; set; }
        public int BundleSize { get; set; }

        [PublicAPI]
        public class Result
        {
            public Guid PromoCodeId { get; init; }
            public string Code { get; set; }
            public double? Amount { get; set; }
            public double? Percent { get; set; }

        }


        internal class PermissionCheck : PermissionCheckBase<GetPromoCodeByCodeCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(GetPromoCodeByCodeCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent()) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<GetPromoCodeByCodeCommand, Result>
        {
	        private readonly ISessionContext sessionContext;
            public Handler(IDb db, ISessionContext sessionContext) : base(db) => this.sessionContext = sessionContext;

            public override async Task<Result> Handle(GetPromoCodeByCodeCommand request, CancellationToken cancellationToken)
            {
                var userId = sessionContext.GetUserId();
                var student = await Db.Students.Where(StudentSpec.ById(userId)).SingleAsync(cancellationToken);
                var promoCode = await Db.PromoCodes.Where(PromoCodeSpec.ByCode(request.PromoCode)).FirstOrDefaultAsync(cancellationToken);
                if (promoCode == null) 
                    throw new ValidationException("Promo code not valid");

                if(!string.IsNullOrEmpty(promoCode.Country) && student.Country != promoCode.Country)
                    throw new ValidationException("Promo code not available in your region");

                if(promoCode.PaymentPeriod != null && promoCode.PaymentPeriod != request.PaymentPeriod)
                    throw new ValidationException($"Promo code not available for {(request.PaymentPeriod == PaymentPeriods.Monthly ? "monthly" : request.PaymentPeriod == PaymentPeriods.Yearly ? "annual" : "")} payments");

                if(promoCode.BundleSize != null && promoCode.BundleSize != request.BundleSize)
                    throw new ValidationException($"Promo code available only for {promoCode.BundleSize} subjects in bundle");

                if (promoCode.PaymentPeriod != null && student.Country != promoCode.Country)
                    throw new ValidationException("Promo code not available in your region");

                if (promoCode.StartAtUtc > DateTime.UtcNow)
                    throw new ValidationException("Promo code not valid");

                if(promoCode.EndAtUtc < DateTime.UtcNow)
                    throw new ValidationException("Promo code was expired");

                if(promoCode.Amount == null && promoCode.Percent == null)
                    throw new ValidationException("Promo code not valid");

                if(promoCode.Percent is > 100)
                    throw new ValidationException("Promo code not valid");

                var userPromoCode = await Db.UserPromoCodes
                    .Where(_ => _.UserId == userId && _.PromoCodeId == promoCode.PromoCodeId)
                    .FirstOrDefaultAsync(cancellationToken);
                if(userPromoCode != null)
                    throw new ValidationException("Promo code already used");

                return new Result
                {
                    PromoCodeId = promoCode.PromoCodeId, Percent = promoCode.Percent, Amount = promoCode.Amount,
                    Code = promoCode.Code
                };
            }
        }
    }
}
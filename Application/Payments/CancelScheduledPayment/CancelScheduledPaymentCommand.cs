using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.CancelScheduledPayment
{
    [PublicAPI]
    public class CancelScheduledPaymentCommand : ICommand<CancelScheduledPaymentCommand.Result>
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }

        [PublicAPI]
        public class Result
        {

		}


		internal class Validator : ValidatorBase<CancelScheduledPaymentCommand>
		{
			public Validator(IDb db) : base(db)
			{
				RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
				RuleFor(_ => _.SubscriptionId).ExistsInTable(Db.ScheduledPayment);
				RuleFor(_ => new { _.UserId, _.SubscriptionId }).ExistsInTable(Db.ScheduledPayment,
					arg => new Spec<ScheduledPaymentEntity>(entity =>
						entity.UserId == arg.UserId && entity.ScheduledPaymentId == arg.SubscriptionId));
			}
		}


		internal class PermissionCheck : PermissionCheckBase<CancelScheduledPaymentCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(CancelScheduledPaymentCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent() || !user.IsAccountOwner(request.UserId)) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<CancelScheduledPaymentCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(CancelScheduledPaymentCommand request, CancellationToken cancellationToken)
            {
	            var schedulePayment = await Db.ScheduledPayment.Where(x => x.UserId == request.UserId && x.ScheduledPaymentId == request.SubscriptionId && x.DeletedAt == null).ToListAsync(cancellationToken);
				schedulePayment.ForEach(x => x.Status = ScheduledPaymentStatus.StoppedByUser);
				await Db.SaveChangesAsync(cancellationToken);

				return new Result{ };
            }

        }
    }
}
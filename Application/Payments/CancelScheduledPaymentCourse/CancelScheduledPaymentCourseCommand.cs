using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.CancelScheduledPaymentCourse
{
    [PublicAPI]
    public class CancelScheduledPaymentCourseCommand : ICommand<CancelScheduledPaymentCourseCommand.Result>
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
        public List<Guid> CourseIds { get; set; }

        [PublicAPI]
        public class Result
        {

		}


		internal class Validator : ValidatorBase<CancelScheduledPaymentCourseCommand>
		{
			public Validator(IDb db) : base(db)
			{
				RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
				RuleFor(_ => _.SubscriptionId).ExistsInTable(Db.ScheduledPayment);
				RuleForEach(_ => _.CourseIds).ExistsInTable(Db.Courses);
				//RuleForEach(_ => _.CourseIds.Select(x => new {CourseId = x, _.SubscriptionId})).ExistsInTable(Db.ScheduledPaymentCourse,
				//	arg => new Spec<ScheduledPaymentCourseEntity>(entity =>
				//		entity.CourseId == arg.CourseId && entity.ScheduledPaymentId == arg.SubscriptionId));
				//RuleFor(_ => new { _.UserId, _.SubscriptionId }).ExistsInTable(Db.ScheduledPayment,
				//	arg => new Spec<ScheduledPaymentEntity>(entity =>
				//		entity.UserId == arg.UserId && entity.ScheduledPaymentId == arg.SubscriptionId));
			}
		}


		internal class PermissionCheck : PermissionCheckBase<CancelScheduledPaymentCourseCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(CancelScheduledPaymentCourseCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent() || !user.IsAccountOwner(request.UserId)) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<CancelScheduledPaymentCourseCommand, Result>
        {
	        private readonly IScheduledPaymentService scheduledPaymentService;

			public Handler(IDb db, IScheduledPaymentService scheduledPaymentService) : base(db) => this.scheduledPaymentService = scheduledPaymentService;

            public override async Task<Result> Handle(CancelScheduledPaymentCourseCommand request, CancellationToken cancellationToken)
            {

				var schedulePayment = await Db.ScheduledPayment
		            .Where(x => x.UserId == request.UserId && x.ScheduledPaymentId == request.SubscriptionId && x.DeletedAt == null)
		            .Include(x => x.ScheduledPaymentCourses)
		            .FirstOrDefaultAsync(cancellationToken);
				
	            foreach (var course in schedulePayment.ScheduledPaymentCourses.Where(x => request.CourseIds.Contains(x.CourseId)))
				{
					if (course.Status != ScheduledPaymentCourseStatus.Active) continue;
					course.Status = ScheduledPaymentCourseStatus.StoppedByUser;
					Db.ScheduledPaymentCourse.Update(course);
				}

	            schedulePayment.Amount = await scheduledPaymentService.GetPrice(schedulePayment);

				// If all subjects cancelled and Subscription is active, cancel subscription too
				if (schedulePayment.ScheduledPaymentCourses.All(x =>
		            x.Status == ScheduledPaymentCourseStatus.StoppedByUser ||
		            x.Status == ScheduledPaymentCourseStatus.Inactive))
	            {
					if(schedulePayment.Status == ScheduledPaymentStatus.Success || schedulePayment.Status == ScheduledPaymentStatus.PaymentFailed)
						schedulePayment.Status = ScheduledPaymentStatus.StoppedByUser;
	            }

				await Db.SaveChangesAsync(cancellationToken);

				return new Result();
            }

        }
    }
}
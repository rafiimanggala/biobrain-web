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
using Biobrain.Domain.Entities.Payment;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.AddScheduledPayment
{
    [PublicAPI]
    public class AddScheduledPaymentCommand : ICommand<AddScheduledPaymentCommand.Result>
    {
	    public PaymentPeriods? Period { get; set; }
	    public List<Guid> Courses { get; set; }
        public Guid UserId { get; set; }
        public Guid? PromoCodeId { get; set; }

		[PublicAPI]
        public class Result
        {

		}


        internal class Validator : ValidatorBase<AddScheduledPaymentCommand>
		{
			public Validator(IDb db) : base(db)
			{
				RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
				RuleFor(_ => _.Period).NotNull();
				RuleFor(_ => _.Courses).NotNull().ChildRules(_ => _.RuleFor(x => x.Count).GreaterThan(0));
			}
		}


		internal class PermissionCheck : PermissionCheckBase<AddScheduledPaymentCommand>
		{
			public PermissionCheck(ISecurityService securityService) : base(securityService)
			{
			}

			protected override bool CanExecute(AddScheduledPaymentCommand request, IUserSecurityInfo user)
			{
				if (!user.IsStudent()) return false;
				if (!user.IsAccountOwner(request.UserId)) return false;
				return true;
			}
		}


		internal class Handler : CommandHandlerBase<AddScheduledPaymentCommand, Result>
        {
	        private readonly IScheduledPaymentService _scheduledPaymentService; 
            public Handler(IDb db, IScheduledPaymentService scheduledPaymentService) : base(db) => _scheduledPaymentService = scheduledPaymentService;

            public override async Task<Result> Handle(AddScheduledPaymentCommand request, CancellationToken cancellationToken)
            {
	            var student = await Db.Users.Where(x => x.Id == request.UserId).Include(x => x.Student).FirstOrDefaultAsync(cancellationToken: cancellationToken);
				
				var scheduledPaymentId = Guid.NewGuid();
				await _scheduledPaymentService.AddScheduledPaymentAndPayAsync(new ScheduledPaymentEntity
				{
					ScheduledPaymentId = scheduledPaymentId,
					Status = ScheduledPaymentStatus.Created,
					Period = request.Period.Value,
					Type = ScheduledPaymentType.Recurring,
					ScheduledPaymentCourses = request.Courses.Select(_ => new ScheduledPaymentCourseEntity
					{
						ScheduledPaymentCourseId = Guid.NewGuid(),
						CourseId = _,
						ScheduledPaymentId = scheduledPaymentId,
						Status = ScheduledPaymentCourseStatus.Active
					}).ToList(),
					UserId = request.UserId
				}, request.UserId, student.Student?.Country, request.PromoCodeId, cancellationToken);

				return new Result();
            }
        }
    }
}
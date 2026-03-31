using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Application.Values;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.GetScheduledPayments
{
    [PublicAPI]
    public class GetScheduledPaymentsQuery : ICommand<List<GetScheduledPaymentsQuery.Result>>
    {
	    public Guid UserId { get; set; }

	    [PublicAPI]
        public class Result
        {
	        public Guid ScheduledPaymentId { get; set; }
	        public List<Course> Courses { get; set; }
	        public PaymentPeriods Period { get; set; }
	        public ScheduledPaymentStatus Status { get; set; }
	        public double Amount { get; set; }
	        public string Currency { get; set; }
	        public DateTime NextPayDateUtc { get; set; }
        }

        public class Course
        {
	        public Guid CourseId { get; set; }
	        public string CourseName { get; set; }
			public ScheduledPaymentCourseStatus Status { get; set; }
        }


        internal class Validator : ValidatorBase<GetScheduledPaymentsQuery>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
	        }
        }


        internal class PermissionCheck : PermissionCheckBase<GetScheduledPaymentsQuery>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(GetScheduledPaymentsQuery request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }

        internal class Handler : CommandHandlerBase<GetScheduledPaymentsQuery, List<Result>>
        {
	        private readonly IPaymentDateService _paymentDateService;

            public Handler(IDb db, IPaymentDateService paymentDateService) : base(db) => _paymentDateService = paymentDateService;

            public override async Task<List<Result>> Handle(GetScheduledPaymentsQuery request, CancellationToken cancellationToken)
            {
	            var subscriptions = await Db.ScheduledPayment
		            .Include(_ => _.ScheduledPaymentCourses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
		            .Where(_ => _.UserId == request.UserId && _.DeletedAt == null
		                        && (_.Status == ScheduledPaymentStatus.Success || _.Status == ScheduledPaymentStatus.StoppedByUser || _.Status == ScheduledPaymentStatus.PaymentFailed)
								&& _.Type == ScheduledPaymentType.Recurring)
		            .OrderByDescending(_ => _.CreatedAt)
		            .Take(10)
		            .ToListAsync(cancellationToken);

	            return subscriptions.Select(_ => new Result
	            {
		            ScheduledPaymentId = _.ScheduledPaymentId,
		            Status = _.Status,
		            Period = _.Period,
		            Amount = _.Amount,
		            Currency = CountryCurrency.GetSymbolByCode(_.Currency),
		            NextPayDateUtc = _paymentDateService.GetNextPayDateUtc(_.PayDate, _.LeapPayDate, _.Period),
		            Courses = _.ScheduledPaymentCourses
			            .Where(_ => _.Status == ScheduledPaymentCourseStatus.Active ||
			                        _.Status == ScheduledPaymentCourseStatus.StoppedByUser)
			            .Select(_ => new Course
			            {
				            CourseId = _.CourseId,
				            CourseName = CourseHelper.GetShortCourseName(_.Course),
				            Status = _.Status
			            }).OrderBy(x => x.CourseName).ToList()
	            }).ToList();
            }
        }
    }
}
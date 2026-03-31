using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using BiobrainWebAPI.Values;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.AddTrialScheduledPayment
{
    [PublicAPI]
    public class AddTrialScheduledPaymentCommand : ICommand<AddTrialScheduledPaymentCommand.Result>
    {
	    public List<Guid> Courses { get; set; }
        public Guid UserId { get; set; }

		[PublicAPI]
        public class Result
        {

		}


        internal class Validator : ValidatorBase<AddTrialScheduledPaymentCommand>
		{
			public Validator(IDb db) : base(db)
			{
				RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
				RuleFor(_ => _.Courses).NotNull().ChildRules(_ => _.RuleFor(x => x.Count).GreaterThan(0));
			}
		}


		internal class PermissionCheck : PermissionCheckBase<AddTrialScheduledPaymentCommand>
        {
            private readonly IDb _db;
			public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(AddTrialScheduledPaymentCommand request, IUserSecurityInfo user)
            {
				if (!user.IsStudent()) return false;
				if (!user.IsAccountOwner(request.UserId)) return false;

                var hasSubscriptions = _db.ScheduledPayment.Any(_ => _.UserId == request.UserId);
                if (hasSubscriptions) return false;
				
                return true;
			}
		}


		internal class Handler : CommandHandlerBase<AddTrialScheduledPaymentCommand, Result>
        {
	        private readonly IPaymentDateService _paymentDateService; 
	        private readonly INotificationService _notificationService; 
	        private readonly ISiteUrls _siteUrls; 
            public Handler(IDb db, IPaymentDateService paymentDateService, INotificationService notificationService, ISiteUrls siteUrls) : base(db)
            {
                _paymentDateService = paymentDateService;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
            }

            public override async Task<Result> Handle(AddTrialScheduledPaymentCommand request, CancellationToken cancellationToken)
            {
	            var student = await Db.Users.Where(x => x.Id == request.UserId).Include(x => x.Student).FirstOrDefaultAsync(cancellationToken: cancellationToken);
				
				var scheduledPaymentId = Guid.NewGuid();
                var scheduledPayment = new ScheduledPaymentEntity
                {
                    ScheduledPaymentId = scheduledPaymentId,
                    Status = ScheduledPaymentStatus.Success,
                    Period = PaymentPeriods.Monthly,
                    Type = ScheduledPaymentType.FreeTrial,
                    ScheduledPaymentCourses = request.Courses.Select(_ => new ScheduledPaymentCourseEntity
                    {
                        ScheduledPaymentCourseId = Guid.NewGuid(),
                        CourseId = _,
                        ScheduledPaymentId = scheduledPaymentId,
                        Status = ScheduledPaymentCourseStatus.Active
                    }).ToList(),
                    UserId = request.UserId,
                    Amount = 0,
                    PayDate = _paymentDateService.GetNotLeapPaydate(DateTime.UtcNow.AddDays(AppSettings.FreeTrialDays)),
                    LeapPayDate = _paymentDateService.GetLeapPaydate(DateTime.UtcNow.AddDays(AppSettings.FreeTrialDays)),
                    Description = $"Free trial subscription for {student.Email}"
                };

                Db.ScheduledPayment.Add(scheduledPayment);
                await Db.SaveChangesAsync(cancellationToken);

                // Send notification
                await _notificationService.Send(new FreeTrialWelcomeNotification(student.Email, student.GetFirstName(), _siteUrls.Login()));

                return new Result();
            }
        }
    }
}
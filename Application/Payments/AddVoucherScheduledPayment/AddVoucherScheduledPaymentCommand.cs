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
using Biobrain.Application.Values;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.Vouchers;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.AddVoucherScheduledPayment
{
    [PublicAPI]
    public class AddVoucherScheduledPaymentCommand : ICommand<AddVoucherScheduledPaymentCommand.Result>
    {
	    public List<Guid> Courses { get; set; }
        public Guid UserId { get; set; }
        public Guid VoucherId { get; set; }

		[PublicAPI]
        public class Result
        {

		}


        internal class Validator : ValidatorBase<AddVoucherScheduledPaymentCommand>
		{
			public Validator(IDb db) : base(db)
			{
				RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.VoucherId).ExistsInTable(Db.Vouchers);
                RuleFor(_ => _.Courses).NotNull().ChildRules(_ => _.RuleFor(x => x.Count).GreaterThan(0));
			}
		}


		internal class PermissionCheck : PermissionCheckBase<AddVoucherScheduledPaymentCommand>
        {
            private readonly IDb _db;
			public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(AddVoucherScheduledPaymentCommand request, IUserSecurityInfo user)
            {
				if (!user.IsStudent()) return false;
				if (!user.IsAccountOwner(request.UserId)) return false;

                var hasSubscriptions = _db.ScheduledPayment.Any(_ => _.UserId == request.UserId);
                if (hasSubscriptions) return false;
				
                return true;
			}
		}


		internal class Handler : CommandHandlerBase<AddVoucherScheduledPaymentCommand, Result>
        {
	        private readonly IPaymentDateService _paymentDateService; 
	        private readonly INotificationService _notificationService; 
	        private readonly ISiteUrls _siteUrls; 
	        private readonly IScheduledPaymentService _paymentService; 
            public Handler(IDb db, IPaymentDateService paymentDateService, INotificationService notificationService, ISiteUrls siteUrls, IScheduledPaymentService paymentService) : base(db)
            {
                _paymentDateService = paymentDateService;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
                _paymentService = paymentService;
            }

            public override async Task<Result> Handle(AddVoucherScheduledPaymentCommand request, CancellationToken cancellationToken)
            {
	            var student = await Db.Users.Where(x => x.Id == request.UserId).Include(x => x.Student).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var userVoucher = await Db.UserVouchers
                    .Include(_ => _.Voucher)
                    .Where(_ => _.VoucherId == request.VoucherId && _.UserId == student.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (userVoucher == null) throw new ValidationException("This voucher not connected with user");
				
				var scheduledPaymentId = Guid.NewGuid();
                var currency = CountryCurrency.Get(student.Student.Country);
                var scheduledPayment = new ScheduledPaymentEntity
                {
                    ScheduledPaymentId = scheduledPaymentId,
                    Status = ScheduledPaymentStatus.Success,
                    Period = PaymentPeriods.Yearly,
                    Type = ScheduledPaymentType.Voucher,
                    Currency = currency.Key,
                    ScheduledPaymentCourses = request.Courses.Select(_ => new ScheduledPaymentCourseEntity
                    {
                        ScheduledPaymentCourseId = Guid.NewGuid(),
                        CourseId = _,
                        ScheduledPaymentId = scheduledPaymentId,
                        Status = ScheduledPaymentCourseStatus.Active
                    }).ToList(),
                    UserId = request.UserId,
                    Amount = 0,
                    PayDate = _paymentDateService.GetNotLeapPaydate(userVoucher.Voucher.ExpiryDateUtc),
                    LeapPayDate = _paymentDateService.GetLeapPaydate(userVoucher.Voucher.ExpiryDateUtc),
                    Description = $"Voucher subscription for {student.Email}"
                };
                var amount = await _paymentService.GetPrice(scheduledPayment);
                if (userVoucher.Voucher.TotalAmount < amount) throw new ValidationException("Voucher amount is not enough");

                await Db.ScheduledPayment.AddAsync(scheduledPayment, cancellationToken);
                userVoucher.Voucher.AmountUsed += amount;

                await Db.UserVoucherTransactions.AddAsync(new UserVoucherTransactionEntity
                {
                    UserVoucherId = userVoucher.UserVoucherId,
                    Amount = amount,
                }, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                // Send notification
                await _notificationService.Send(new WelcomeStudentNotification(student.Email, student.GetFirstName(), _siteUrls.Login()));

                return new Result();
            }
        }
    }
}
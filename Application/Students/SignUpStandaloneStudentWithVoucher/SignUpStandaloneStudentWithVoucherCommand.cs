using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Student;
using Biobrain.Domain.Entities.Vouchers;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.SignUpStandaloneStudentWithVoucher
{
    [PublicAPI]
    public class SignUpStandaloneStudentWithVoucherCommand : ICommand<SignUpStandaloneStudentWithVoucherCommand.Result>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Voucher { get; init; }
        public string SchoolName { get; init; }
        public string Country { get; init; }
        public string State { get; set; }
        public int? CurriculumCode { get; set; }
        public int? Year { get; set; }

        private ICreateAccountService.Request ToCreateAccountRequest(params string[] roles) => new()
                                                                                               {
                                                                                                   UserName = Email,
                                                                                                   Email = Email,
                                                                                                   Password = Password,
                                                                                                   Roles = roles.ToImmutableArray()
                                                                                               };


        [PublicAPI]
        public class Result
        {
            public Guid StudentId { get; set; }
        }


        internal class Validator : ValidatorBase<SignUpStandaloneStudentWithVoucherCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail).WithMessage("Email already exists. Please login and enter Access code using \"Add subject\" from account menu.");
                RuleFor(_ => _.Password).NotEmpty();
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
                RuleFor(_ => _.Country).NotEmpty();
                RuleFor(_ => _.SchoolName).NotEmpty();
                RuleFor(_ => _.Voucher).NotEmpty().ExistsInTable(Db.Vouchers, VoucherCodeSpec.ByVoucher).WithMessage("Voucher not found");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SignUpStandaloneStudentWithVoucherCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SignUpStandaloneStudentWithVoucherCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SignUpStandaloneStudentWithVoucherCommand, Result>
        {
            private readonly ICreateAccountService _createAccountService;

            public Handler(IDb db, ICreateAccountService createAccountService) : base(db) => _createAccountService = createAccountService;

            public override async Task<Result> Handle(SignUpStandaloneStudentWithVoucherCommand request, CancellationToken cancellationToken)
            {
                var voucher = await Db.Vouchers
                    .Where(VoucherCodeSpec.ByVoucher(request.Voucher))
                    .SingleAsync(cancellationToken);
                var userVoucher = await Db.UserVouchers.AsNoTracking().Where(_ => _.VoucherId == voucher.VoucherId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (voucher.RedeemExpiryDateUtc < DateTime.UtcNow) throw new ValidationException("Voucher is out of date");
                if (!String.Equals(voucher.Country, request.Country, StringComparison.InvariantCultureIgnoreCase)) throw new ValidationException($"Voucher is only for {voucher.Country}");
                if (userVoucher != null) throw new ValidationException($"This voucher was already activated");

                try
                {
                    await Db.BeginTransactionAsync(cancellationToken);

                    var user = await _createAccountService.Create(
                        request.ToCreateAccountRequest(Constant.Roles.Student), cancellationToken);

                    var student = new StudentEntity
                    {
                        StudentId = user.Id,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Country = request.Country,
                        State = request.State,
                        CurriculumCode = request.CurriculumCode,
                        Year = request.Year
                    };
                    await Db.Students.AddAsync(student, cancellationToken);

                    //var scheduledPaymentId = Guid.NewGuid();
                    //var scheduledPayment = new ScheduledPaymentEntity
                    //{
                    //    ScheduledPaymentId = scheduledPaymentId,
                    //    Status = ScheduledPaymentStatus.Success,
                    //    Period = PaymentPeriods.Yearly,
                    //    Type = ScheduledPaymentType.AccessCode,
                    //    ScheduledPaymentCourses = voucher.Batch.Courses.Select(_ => new ScheduledPaymentCourseEntity
                    //    {
                    //        ScheduledPaymentCourseId = Guid.NewGuid(),
                    //        CourseId = _.CourseId,
                    //        ScheduledPaymentId = scheduledPaymentId,
                    //        Status = ScheduledPaymentCourseStatus.Active
                    //    }).ToList(),
                    //    UserId = user.Id,
                    //    Amount = 0,
                    //    PayDate = _paymentDateService.GetNotLeapPaydate(
                    //        DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                    //    LeapPayDate =
                    //        _paymentDateService.GetLeapPaydate(DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                    //    Description = $"Access code subscription for {request.Email} with code {request.AccessCode}",
                    //    ExpiryDate = voucher.Batch.ExpiryDate,
                    //};

                    //Db.ScheduledPayment.Add(scheduledPayment);

                    //await Db.AccessCodeMilestone.AddAsync(
                    //    new AccessCodeMilestoneEntity
                    //        { Code = voucher.Code, BatchId = voucher.BatchId, UserId = user.Id },
                    //    cancellationToken);
                    //Db.AccessCodes.Remove(voucher);
                    
                    // Add User Voucher
                    await Db.UserVouchers.AddAsync(new UserVoucherEntity
                    {
                        VoucherId = voucher.VoucherId,
                        UserId = user.Id,
                        SchoolName = request.SchoolName,
                    }, cancellationToken);


                    await Db.SaveChangesAsync(cancellationToken);

                    await Db.CommitTransactionAsync();

                    return new Result { StudentId = student.StudentId };
                }
                catch (Exception)
                {
                    await Db.RollbackTransactionAsync();
                    throw;
                }
            }
        }
    }
}

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.AccessCodes;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.Student;
using BiobrainWebAPI.Values;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.SignUpStandaloneStudentWithAccessCode
{
    [PublicAPI]
    public class SignUpStandaloneStudentWithAccessCodeCommand : ICommand<SignUpStandaloneStudentWithAccessCodeCommand.Result>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string AccessCode { get; init; }
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


        internal class Validator : ValidatorBase<SignUpStandaloneStudentWithAccessCodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail).WithMessage("Email already exists. Please login and enter Access code using \"Add subject\" from account menu.");
                RuleFor(_ => _.Password).NotEmpty();
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
                RuleFor(_ => _.Country).NotEmpty();
                RuleFor(_ => _.AccessCode).NotEmpty().ExistsInTable(Db.AccessCodes, AccessCodeSpec.ByAccessCode).WithMessage("Access Code not found");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SignUpStandaloneStudentWithAccessCodeCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SignUpStandaloneStudentWithAccessCodeCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SignUpStandaloneStudentWithAccessCodeCommand, Result>
        {
            private readonly ICreateAccountService _createAccountService;
            private readonly IPaymentDateService _paymentDateService;

            public Handler(IDb db, ICreateAccountService createAccountService, IPaymentDateService paymentDateService) : base(db)
            {
                _createAccountService = createAccountService;
                _paymentDateService = paymentDateService;
            }
            
            public override async Task<Result> Handle(SignUpStandaloneStudentWithAccessCodeCommand request, CancellationToken cancellationToken)
            {
                var accessCode = await Db.AccessCodes
                    .Include(_ => _.Batch).ThenInclude(_ => _.Courses)
                    .Where(AccessCodeSpec.ByAccessCode(request.AccessCode))
                    .SingleAsync(cancellationToken);
                if (accessCode.Batch.ExpiryDate < DateTime.UtcNow) throw new ValidationException("Access Code is out of date");

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

                    var scheduledPaymentId = Guid.NewGuid();
                    var scheduledPayment = new ScheduledPaymentEntity
                    {
                        ScheduledPaymentId = scheduledPaymentId,
                        Status = ScheduledPaymentStatus.Success,
                        Period = PaymentPeriods.Yearly,
                        Type = ScheduledPaymentType.AccessCode,
                        ScheduledPaymentCourses = accessCode.Batch.Courses.Select(_ => new ScheduledPaymentCourseEntity
                        {
                            ScheduledPaymentCourseId = Guid.NewGuid(),
                            CourseId = _.CourseId,
                            ScheduledPaymentId = scheduledPaymentId,
                            Status = ScheduledPaymentCourseStatus.Active
                        }).ToList(),
                        UserId = user.Id,
                        Amount = 0,
                        PayDate = _paymentDateService.GetNotLeapPaydate(
                            DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                        LeapPayDate =
                            _paymentDateService.GetLeapPaydate(DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                        Description = $"Access code subscription for {request.Email} with code {request.AccessCode}",
                        ExpiryDate = accessCode.Batch.ExpiryDate,
                    };

                    Db.ScheduledPayment.Add(scheduledPayment);

                    await Db.AccessCodeMilestone.AddAsync(
                        new AccessCodeMilestoneEntity
                            { Code = accessCode.Code, BatchId = accessCode.BatchId, UserId = user.Id },
                        cancellationToken);
                    Db.AccessCodes.Remove(accessCode);

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

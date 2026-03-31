using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.AccessCodes;
using Biobrain.Domain.Entities.Payment;
using BiobrainWebAPI.Values;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.UseAccessCode
{
    [PublicAPI]
    public class UseAccessCodeCommand : ICommand<UseAccessCodeCommand.Result>
    {
        public Guid StudentId { get; init; }
        public string AccessCode { get; init; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UseAccessCodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
	            RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.AccessCode).NotEmpty().ExistsInTable(Db.AccessCodes, AccessCodeSpec.ByAccessCode);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UseAccessCodeCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UseAccessCodeCommand request, IUserSecurityInfo user)
            {
                if (!user.IsAccountOwner(request.StudentId)) return false;

                return true;
            }
        }


        internal class Handler : CommandHandlerBase<UseAccessCodeCommand, Result>
        {
            private readonly IPaymentDateService _paymentDateService;
            public Handler(IDb db, IPaymentDateService paymentDateService) : base(db) => _paymentDateService = paymentDateService;

            public override async Task<Result> Handle(UseAccessCodeCommand request, CancellationToken cancellationToken)
            {
                var student = await Db.Students
                    .Include(_ => _.User)
                                      .GetSingleAsync(StudentSpec.ById(request.StudentId), cancellationToken);

                var accessCode = await Db.AccessCodes
                    .Include(_ => _.Batch).ThenInclude(_ => _.Courses)
                    .Where(AccessCodeSpec.ByAccessCode(request.AccessCode))
	                .GetSingleAsync(cancellationToken);
                if (accessCode.Batch.ExpiryDate < DateTime.UtcNow) throw new ValidationException("Access Code is out of date");

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
                    UserId = student.StudentId,
                    Amount = 0,
                    PayDate = _paymentDateService.GetNotLeapPaydate(
                        DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                    LeapPayDate =
                        _paymentDateService.GetLeapPaydate(DateTime.UtcNow.AddMonths(AppSettings.AccessCodeMonth)),
                    Description = $"Access code subscription for {student.User.Email} with code {request.AccessCode}",
                    ExpiryDate = accessCode.Batch.ExpiryDate,
                };

                Db.ScheduledPayment.Add(scheduledPayment);

                await Db.AccessCodeMilestone.AddAsync(
                    new AccessCodeMilestoneEntity
                        { Code = accessCode.Code, BatchId = accessCode.BatchId, UserId = student.StudentId },
                    cancellationToken);
                Db.AccessCodes.Remove(accessCode);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result{ };
            }

        }
    }
}

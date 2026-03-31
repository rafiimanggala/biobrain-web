using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.DbRequests;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Application.Students;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.InviteByEmailToSchoolClass
{
    [PublicAPI]
    public class InviteByEmailToSchoolClassCommand : ICommand<InviteByEmailToSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public string Email { get; init; }


        [PublicAPI]
        public class Result
        {
            public required bool IsStudentAddedToClass { get; init; }
        }


        internal class Validator : ValidatorBase<InviteByEmailToSchoolClassCommand>
        {
            public Validator(IDb db, UserManager<UserEntity> userManager) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                //RuleFor(_ => _.Email).MustAsync(async (command, _, _) => (await userManager.FindUserByLoginName(command.Email)) == null).WithErrorCode("User with this email already exist");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<InviteByEmailToSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(InviteByEmailToSchoolClassCommand request, IUserSecurityInfo user)
            {
                //if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<InviteByEmailToSchoolClassCommand, Result>
        {
            private readonly INotificationService _notificationService;
            private readonly ISessionContext _sessionContext;
            private readonly ISiteUrls _siteUrls;
            private readonly IJoinStudentToSchoolClassWithAccessCodeService _joinStudentToSchoolClassWithAccessCodeService;

            public Handler(IDb db,
                           INotificationService notificationService,
                           ISessionContext sessionContext,
                           ISiteUrls siteUrls,
                           IJoinStudentToSchoolClassWithAccessCodeService joinStudentToSchoolClassWithAccessCodeService)
                : base(db)
            {
	            _notificationService = notificationService;
	            _sessionContext = sessionContext;
	            _siteUrls = siteUrls;
                _joinStudentToSchoolClassWithAccessCodeService = joinStudentToSchoolClassWithAccessCodeService;
            }

            public override async Task<Result> Handle(InviteByEmailToSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses.AsNoTracking()
                    .Include(_ => _.School)
                    .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);
                var teacher = await Db.Users.AsNoTracking()
	                .Where(UserSpec.ById(_sessionContext.GetUserId()))
	                .FirstOrDefaultAsync(cancellationToken);
                var user = await Db.Users.AsNoTracking()
	                .Include(x => x.Student)
	                .Include(x => x.Student.SchoolClasses)
                    .Include(x => x.Teacher)
                    .Where(UserSpec.WithLoginNameOrEmail(request.Email))
	                .FirstOrDefaultAsync(cancellationToken);

                // Add user to school if not in yet
                if (user?.Student == null)
                {
                    if (user?.Teacher != null)
                        throw new ValidationException("This user already registered as teacher.");

                    await SendInviteByEmailNotificationAsync(request, teacher, schoolClass);

                    return new Result { IsStudentAddedToClass = false };
                }

                if (!schoolClass.School.UseAccessCodes)
                {
                    await AssignToSchoolWithoutAccessCode(request, schoolClass, user, cancellationToken);
                    return new Result { IsStudentAddedToClass = true };
                }

                await _joinStudentToSchoolClassWithAccessCodeService.Perform(schoolClass, user.Student, cancellationToken);
                return new Result { IsStudentAddedToClass = true };
            }

            private async Task AssignToSchoolWithoutAccessCode(InviteByEmailToSchoolClassCommand request,
                                                               SchoolClassEntity schoolClass,
                                                               UserEntity user,
                                                               CancellationToken ct)
            {
                if (Db.Students.CheckLicenseOverflowForSchool(schoolClass.School, 1))
                    throw new NotEnoughStudentsLicensesException();

                var userSchool = await Db.SchoolStudents.AsNoTracking()
                                         .Where(x => x.SchoolId == schoolClass.SchoolId && x.StudentId == user.Id)
                                         .FirstOrDefaultAsync(ct);
                if (userSchool == null)
                {
                    await Db.SchoolStudents.AddAsync(
                        new SchoolStudentEntity { SchoolId = schoolClass.SchoolId, StudentId = user.Id },
                        ct);
                    await Db.SaveChangesAsync(ct);
                }

                var userClass = await Db.SchoolClassStudents.AsNoTracking()
                                        .Where(x => x.SchoolClassId == request.SchoolClassId && x.StudentId == user.Id)
                                        .FirstOrDefaultAsync(ct);
                if (userClass == null)
                {
                    await Db.SchoolClassStudents.AddAsync(
                        new SchoolClassStudentEntity { SchoolClassId = schoolClass.SchoolClassId, StudentId = user.Id },
                        ct);
                    await Db.SaveChangesAsync(ct);
                }
            }

            private Task SendInviteByEmailNotificationAsync(InviteByEmailToSchoolClassCommand request, UserEntity teacher, SchoolClassEntity schoolClass)
            {
                // Send notification
                InviteByEmailNotification notification = new(request.Email,
                                                             teacher.GetFullName(),
                                                             $"{schoolClass.Year} {schoolClass.Name}",
                                                             schoolClass.AutoJoinClassCode,
                                                             _siteUrls.Login());
                return _notificationService.Send(notification);
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("BED4396C-65C2-4C7E-91E2-18EAB8F4C2D6");
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;
using Biobrain.Domain.Entities.Teacher;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Teachers.CreateTeacher
{
    [PublicAPI]
    public partial class CreateTeacherCommand : ICommand<CreateTeacherCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid TeacherId { get; set; }
            public Guid SchoolId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }


        internal class Validator : ValidatorBase<CreateTeacherCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail);
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateTeacherCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateTeacherCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<CreateTeacherCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;
            public Handler(IDb db,  UserManager<UserEntity> userManager, INotificationService notificationService, ISiteUrls siteUrls) : base(db)
            {
                _userManager = userManager;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
            }

            public override async Task<Result> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                if (Db.Teachers.Include(x => x.Schools).Count(TeacherSpec.ForSchool(request.SchoolId)) + 1 > school.TeachersLicensesNumber)
                    throw new NotEnoughTeachersLicensesException();

                var user = new UserEntity { UserName = request.Email, Email = request.Email };

                var userValidationResult = await _userManager.ValidateUser(user);
                if (!userValidationResult.IsValid)
                    throw new ValidationException(userValidationResult.Errors);

                await _userManager.CreateUser(user);
                await _userManager.AssignUserToRoles(user, Constant.Roles.Teacher);


                var teacher = new TeacherEntity
                              {
                                  TeacherId = user.Id,
                                  FirstName = request.FirstName,
                                  LastName = request.LastName,
                              };
                await Db.Teachers.AddAsync(teacher, cancellationToken);
                await Db.SchoolTeachers.AddAsync(new SchoolTeacherEntity
                {
                    SchoolId = request.SchoolId,
                    TeacherId = teacher.TeacherId
                }, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _notificationService.Send(new TeacherCreatedNotification(user.Email, teacher.GetFullName(), _siteUrls.Login(), _siteUrls.SetPasswordAfterRegistration(user.UserName, token)));

                return new Result
                       {
                           TeacherId = teacher.TeacherId,
                           SchoolId = request.SchoolId,
                           FirstName = teacher.FirstName,
                           LastName = teacher.LastName
                       };
            }
        }
    }
}

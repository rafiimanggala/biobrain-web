using System;
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
using Biobrain.Domain.Entities.Student;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Students.CreateStudent
{
    [PublicAPI]
    public partial class CreateStudentCommand : ICommand<CreateStudentCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; init; }
        public string State { get; set; }
        public int CurriculumCode { get; set; }
        public int Year { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid StudentId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateStudentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail);
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
                RuleFor(_ => _.Country).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateStudentCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateStudentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<CreateStudentCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;

            public Handler(IDb db, UserManager<UserEntity> userManager, INotificationService notificationService, ISiteUrls siteUrls) : base(db)
            {
                _userManager = userManager;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
            }

            public override async Task<Result> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                if (school.UseAccessCodes) throw new ValidationException("This function is unavailable for schools that use Access Codes license type");
                //if (Db.Students.CheckLicenseForSchool(school))
                //    throw new NotEnoughStudentsLicensesException();

                var user = new UserEntity { UserName = request.Email, Email = request.Email };
                
                var userValidationResult = await _userManager.ValidateUser(user);
                if (!userValidationResult.IsValid)
                    throw new ValidationException(userValidationResult.Errors);

                await _userManager.CreateUser(user);
                await _userManager.AssignUserToRoles(user, Constant.Roles.Student);


                var student = new StudentEntity
                              {
                                  StudentId = user.Id,
                                  FirstName = request.FirstName, 
                                  LastName = request.LastName,
                                  Country = request.Country,
                                  CurriculumCode = request.CurriculumCode,
                                  State = request.State,
                                  Year = request.Year,
                              };

                await Db.Students.AddAsync(student, cancellationToken);
                await Db.SchoolStudents.AddAsync(new SchoolStudentEntity{StudentId = student.StudentId, SchoolId = request.SchoolId}, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _notificationService.Send(new StudentCreatedNotification(user.Email, student.GetFullName(), _siteUrls.Login(), _siteUrls.SetPasswordAfterRegistration(user.UserName, token)));

                return new Result {StudentId = student.StudentId};
            }
        }
    }
}

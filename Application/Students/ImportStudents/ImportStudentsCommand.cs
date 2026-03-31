using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.DbRequests;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;
using Biobrain.Domain.Entities.Student;
using BiobrainWebAPI.Values;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Students.ImportStudents
{
    [PublicAPI]
    public class ImportStudentsCommand : ICommand<ImportStudentsCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public Guid SchoolClassId { get; set; }
        public List<StudentImportModel> Students { get; set; } = new();

        [PublicAPI]
        public class StudentImportModel
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }


        [PublicAPI]
        public record Result(List<ResultItem> Results);

        public record ResultItem(StudentImportModel Model, StudentImportResult ImportResult);

        public record StudentImportResult(StudentCreatedResult Created, StudentUpdatedResult Updated, string Error);

        public record StudentCreatedResult(Guid StudentId);

        public record StudentUpdatedResult(Guid StudentId);


        internal class Validator : ValidatorBase<ImportStudentsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);

                RuleForEach(_ => _.Students).ChildRules(s =>
                {
                    s.RuleFor(_ => _.Email).NotEmpty().EmailAddress();
                    s.RuleFor(_ => _.FirstName).NotEmpty();
                    s.RuleFor(_ => _.LastName).NotEmpty();
                });
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ImportStudentsCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(ImportStudentsCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsSchoolAdmin(request.SchoolId);
        }


        internal class Handler : CommandHandlerBase<ImportStudentsCommand, Result>
        {
            private readonly UserManager<UserEntity> userManager;
            private readonly INotificationService notificationService;
            private readonly ISiteUrls siteUrls;
            private readonly ILogger<Handler> logger;

            public Handler(IDb db, UserManager<UserEntity> userManager, INotificationService notificationService, ISiteUrls siteUrls, ILogger<Handler> logger) : base(db)
            {
                this.userManager = userManager;
                this.notificationService = notificationService;
                this.siteUrls = siteUrls;
                this.logger = logger;
            }

            public override async Task<Result> Handle(ImportStudentsCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);

                if (Db.Students.CheckLicenseOverflowForSchool(school, request.Students.Count))
	                throw new NotEnoughStudentsLicensesException();

                var results = new List<ResultItem>();
                foreach (var studentImportModel in request.Students) 
                    results.Add(new ResultItem(studentImportModel, await ImportStudent(request, studentImportModel)));

                return new Result(results);
            }

            private async Task<StudentImportResult> ImportStudent(ImportStudentsCommand request, StudentImportModel studentImportModel)
            {
                try
                {
                    var studentUpdatedResult = await UpdateStudent(request, studentImportModel);
                    if (studentUpdatedResult != null)
                        return new StudentImportResult(null, studentUpdatedResult, null);

                    var studentCreatedResult = await CreateStudent(request, studentImportModel);
                    if (studentCreatedResult != null)
                        return new StudentImportResult(studentCreatedResult, null, null);
                }
                catch (BusinessLogicException e)
                {
                    return new StudentImportResult(null, null, e.Message);
                }
                catch (ValidationException e)
                {
                    return new StudentImportResult(null, null, e.Message);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Can not import user.");
                    return new StudentImportResult(null, null, e.Message);
                }

                return new StudentImportResult(null, null, null);
            }

            private async Task<StudentUpdatedResult> UpdateStudent(ImportStudentsCommand request, StudentImportModel studentImportModel)
            {
                var existingUser = await userManager.FindUserByLoginName(studentImportModel.Email);
                if (existingUser == null)
                    return null;

                var existingStudent = await Db.Students.Include(_ => _.Schools).Include(_ => _.SchoolClasses)
	                .SingleOrDefaultAsync(StudentSpec.ById(existingUser.Id));
                if (existingStudent == null)
                    throw new EmailIsNotAssignedToStudentsAccountException();

                if (!existingStudent.IsInSchool(request.SchoolId))
	                await Db.SchoolStudents.AddAsync(new SchoolStudentEntity{SchoolId = request.SchoolId, StudentId = existingStudent.StudentId});
                if (!existingStudent.IsInSchoolClass(request.SchoolClassId))
                {
	                var schoolClassStudents = new SchoolClassStudentEntity
	                {
		                StudentId = existingStudent.StudentId,
		                SchoolClassId = request.SchoolClassId
	                };
	                await Db.SchoolClassStudents.AddAsync(schoolClassStudents);
                }

                existingStudent.FirstName = studentImportModel.FirstName;
                existingStudent.LastName = studentImportModel.LastName;

                await Db.SaveChangesAsync();

                return new StudentUpdatedResult(existingStudent.StudentId);
            }

            private async Task<StudentCreatedResult> CreateStudent(ImportStudentsCommand request, StudentImportModel studentImportModel)
            {
                var user = new UserEntity { UserName = studentImportModel.Email, Email = studentImportModel.Email };

                var userValidationResult = await userManager.ValidateUser(user);
                if (!userValidationResult.IsValid)
                    throw new ValidationException(userValidationResult.Errors);

                await userManager.CreateUser(user);
                await userManager.AssignUserToRoles(user, Constant.Roles.Student);

                var student = new StudentEntity
                              {
                                  StudentId = user.Id,
                                  //SchoolId = request.SchoolId,
                                  FirstName = studentImportModel.FirstName,
                                  LastName = studentImportModel.LastName,
                                  Country = AppSettings.Students.DefaultCountry,
                                  State = AppSettings.Students.DefaultState,
                                  CurriculumCode = AppSettings.Students.DefaultCurriculumCode,
                              };

                await Db.Students.AddAsync(student);

                await Db.SchoolStudents.AddAsync(new SchoolStudentEntity{SchoolId = request.SchoolId, StudentId = student.StudentId});

                var schoolClassStudents = new SchoolClassStudentEntity
                                          {
                                              StudentId = student.StudentId,
                                              SchoolClassId = request.SchoolClassId
                                          };
                await Db.SchoolClassStudents.AddAsync(schoolClassStudents);

                await Db.SaveChangesAsync();

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                await notificationService.Send(new StudentCreatedNotification(user.Email, student.GetFullName(), siteUrls.Login(), siteUrls.SetPasswordAfterRegistration(user.UserName, token)));

                return new StudentCreatedResult(student.StudentId);
            }
        }

        public class EmailIsNotAssignedToStudentsAccountException : BusinessLogicException
        {
            public override Guid ErrorCode => new("D64834AF-3E14-4919-9D71-8C71AC3E526A");
        }

        public class EmailIsAssignedToStudentFromAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("BEAD041E-ED7B-4B2B-B3DF-5A09FD5C9820");
        }
    }
}

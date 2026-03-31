using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.DbRequests;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.Student;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Students.SignUpStudent
{
    [PublicAPI]
    public class SignUpStudentCommand : ICommand<SignUpStudentCommand.Result>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string ClassCode { get; init; }
        public string Country { get; init; }
        public string State { get; set; }
        public int? CurriculumCode { get; set; }

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
            public Guid SchoolId { get; set; }
            public Guid SchoolClassId { get; set; }
            public Guid StudentId { get; set; }
        }


        internal class Validator : ValidatorBase<SignUpStudentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail).WithMessage("Email already exists");
                RuleFor(_ => _.Password).NotEmpty();
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
                RuleFor(_ => _.ClassCode).NotEmpty().ExistsInTable(Db.SchoolClasses, SchoolClassSpec.ByClassCode);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SignUpStudentCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SignUpStudentCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SignUpStudentCommand, Result>
        {
            private readonly ICreateAccountService _createAccountService;

            public Handler(IDb db, ICreateAccountService createAccountService) : base(db) => _createAccountService = createAccountService;

            public override async Task<Result> Handle(SignUpStudentCommand request, CancellationToken cancellationToken)
            {
	            var schoolClass = await Db.SchoolClasses
		            .Include(x => x.School)
		            .Where(SchoolClassSpec.ByClassCode(request.ClassCode))
		            .GetSingleAsync(cancellationToken);

                if (Db.Students.CheckLicenseOverflowForSchool(schoolClass.School))
	                throw new NotEnoughStudentsLicensesException();
                if (schoolClass.School.UseAccessCodes)
                    throw new ClassCodeNotAvailableException();
                var user = await _createAccountService.Create(request.ToCreateAccountRequest(Constant.Roles.Student), cancellationToken);

                var student = new StudentEntity
                              {
                                  StudentId = user.Id,
                                  FirstName = request.FirstName,
                                  LastName = request.LastName,
                                  Country = request.Country,
                                  State = request.State,
                                  CurriculumCode = request.CurriculumCode
                                  //SchoolId = schoolClass.SchoolId,
                              };
                await Db.Students.AddAsync(student, cancellationToken);

                var schoolClassStudents = new SchoolClassStudentEntity
                                          {
                                              StudentId = student.StudentId,
                                              SchoolClassId = schoolClass.SchoolClassId
                                          };
                await Db.SchoolClassStudents.AddAsync(schoolClassStudents, cancellationToken);
                if (!await Db.SchoolStudents.AnyAsync(_ => _.StudentId == student.StudentId && _.SchoolId == schoolClass.SchoolId, cancellationToken))
	                await Db.SchoolStudents.AddAsync(new SchoolStudentEntity { SchoolId = schoolClass.SchoolId, StudentId = student.StudentId }, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                       {
                           SchoolId = schoolClass.SchoolId,
                           SchoolClassId = schoolClass.SchoolClassId,
                           StudentId = student.StudentId
                       };
            }
        }
    }
}

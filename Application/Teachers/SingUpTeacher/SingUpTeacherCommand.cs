using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.Teacher;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Teachers.SingUpTeacher
{
    [PublicAPI]
    public class SignUpTeacherCommand : ICommand<SignUpTeacherCommand.Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

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
            public Guid TeacherId { get; set; }
        }


        internal class Validator : ValidatorBase<SignUpTeacherCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail);
                RuleFor(_ => _.Password).NotEmpty();
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SignUpTeacherCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SignUpTeacherCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SignUpTeacherCommand, Result>
        {
            private readonly ICreateAccountService _createAccountService;

            public Handler(IDb db, ICreateAccountService createAccountService) : base(db) => _createAccountService = createAccountService;

            public override async Task<Result> Handle(SignUpTeacherCommand request, CancellationToken cancellationToken)
            {
                var user = await _createAccountService.Create(request.ToCreateAccountRequest(Constant.Roles.Teacher, Constant.Roles.SchoolAdministrator), cancellationToken);

                var school = new SchoolEntity { Name = $"{request.FirstName} {request.LastName}", TeachersLicensesNumber = 1, StudentsLicensesNumber = 100 };
                await Db.Schools.AddAsync(school, cancellationToken);

                var teacher = new TeacherEntity {TeacherId = user.Id, FirstName = request.FirstName, LastName = request.LastName};
                await Db.Teachers.AddAsync(teacher, cancellationToken);
                await Db.SchoolTeachers.AddAsync(new SchoolTeacherEntity{TeacherId = teacher.TeacherId, SchoolId = school.SchoolId }, cancellationToken);

                var schoolAdmin = new SchoolAdminEntity {SchoolId = school.SchoolId, TeacherId = teacher.TeacherId};
                await Db.SchoolAdmins.AddAsync(schoolAdmin, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                       {
                           SchoolId = school.SchoolId,
                           TeacherId = teacher.TeacherId
                       };
            }
        }
    }
}

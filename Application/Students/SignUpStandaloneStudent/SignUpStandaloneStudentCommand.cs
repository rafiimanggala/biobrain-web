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
using Biobrain.Domain.Entities.Student;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Students.SignUpStandaloneStudent
{
    [PublicAPI]
    public class SignUpStandaloneStudentCommand : ICommand<SignUpStandaloneStudentCommand.Result>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
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


        internal class Validator : ValidatorBase<SignUpStandaloneStudentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail).WithMessage("Email already exists");
                RuleFor(_ => _.Password).NotEmpty();
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
                RuleFor(_ => _.Country).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SignUpStandaloneStudentCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SignUpStandaloneStudentCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SignUpStandaloneStudentCommand, Result>
        {
            private readonly ICreateAccountService _createAccountService;

            public Handler(IDb db, ICreateAccountService createAccountService) : base(db) => _createAccountService = createAccountService;

            public override async Task<Result> Handle(SignUpStandaloneStudentCommand request, CancellationToken cancellationToken)
            {
	            var user = await _createAccountService.Create(request.ToCreateAccountRequest(Constant.Roles.Student), cancellationToken);

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

                await Db.SaveChangesAsync(cancellationToken);

                return new Result{StudentId = student.StudentId};
            }
        }
    }
}

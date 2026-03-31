using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class ChangeSelfEmailCommand : ICommand<ChangeSelfEmailCommand.Result>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ChangeSelfEmailCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).NotEmpty().ExistsInTable(Db.Users);
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail).WithMessage("Email already exists");
                RuleFor(_ => _.Password).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ChangeSelfEmailCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(ChangeSelfEmailCommand request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<ChangeSelfEmailCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;

            public Handler(IDb db, UserManager<UserEntity> userManager) : base(db) => _userManager = userManager;

            public override async Task<Result> Handle(ChangeSelfEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserById(request.UserId);

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                    throw new IncorrectPasswordException();

                user.UserName = request.Email;
                user.Email = request.Email;
                
                var result = await _userManager.UpdateAsync(user);
                if (result.Errors.Any())
                    throw new CanNotChangeUserEmailException(result.Errors);

                return new Result();
            }
        }
    }
}
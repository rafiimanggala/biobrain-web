using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class ChangePasswordCommand : ICommand<ChangePasswordCommand.Result>
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ChangePasswordCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.NewPassword).NotEmpty();
            }
        }
        

        internal class PermissionCheck : PermissionCheckBase<ChangePasswordCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(ChangePasswordCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<ChangePasswordCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;

            public Handler(IDb db, UserManager<UserEntity> userManager) : base(db) => this._userManager = userManager;

            public override async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserById(request.UserId);
                
                var validationResult = await _userManager.ValidatePassword(nameof(request.NewPassword), request.NewPassword, user);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var result = await _userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                    throw new CanNotChangePasswordException(result.Errors);

                result = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (!result.Succeeded)
                    throw new CanNotChangePasswordException(result.Errors);

                return new Result();
            }
        }
    }
}

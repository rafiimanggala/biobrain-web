using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.ResetPassword;
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
    public class SetPasswordCommand : ICommand<SetPasswordCommand.Result>
    {
        public string LoginName { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<SetPasswordCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LoginName).NotEmpty().ExistsInTable(Db.Users, UserSpec.WithLoginName);
                RuleFor(_ => _.Token).NotEmpty();
                RuleFor(_ => _.NewPassword).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SetPasswordCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SetPasswordCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<SetPasswordCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;

            public Handler(IDb db, UserManager<UserEntity> userManager) : base(db) => this._userManager = userManager;

            public override async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserByLoginName(request.LoginName);

                var validationResult = await _userManager.ValidatePassword(nameof(request.NewPassword), request.NewPassword, user);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (!result.Succeeded) 
                    throw new CanNotSetPasswordException(result.Errors);

                return new Result();
            }
        }
    }
}

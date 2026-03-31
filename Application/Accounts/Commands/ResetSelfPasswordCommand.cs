using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class ResetSelfPasswordCommand : ICommand<ResetSelfPasswordCommand.Result>
    {
        public string LoginName { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ResetSelfPasswordCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LoginName).NotEmpty().ExistsInTable(Db.Users, UserSpec.WithLoginName);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ResetSelfPasswordCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(ResetSelfPasswordCommand request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<ResetSelfPasswordCommand, Result>
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

            public override async Task<Result> Handle(ResetSelfPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserByLoginName(request.LoginName);

                if (string.IsNullOrEmpty(user.Email))
                    throw new UserHasNoEmailException();

                var studentOrTeacher = await Db.Users
                                               .Include(_ => _.Student)
                                               .Include(_ => _.Teacher)
                                               .SingleOrDefaultAsync(UserSpec.ById(user.Id), cancellationToken);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _notificationService.Send(new ResetSelfPasswordNotification(user.Email, studentOrTeacher.GetFullName(), _siteUrls.SetPassword(user.UserName, token)));

                return new Result();
            }
        }
    }
}
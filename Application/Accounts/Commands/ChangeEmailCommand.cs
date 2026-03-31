using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
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
    public class ChangeEmailCommand : ICommand<ChangeEmailCommand.Result>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ChangeEmailCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).NotEmpty().ExistsInTable(Db.Users);
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().Unique(Db.Users, UserSpec.WithLoginNameOrEmail);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ChangeEmailCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(ChangeEmailCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var student = _db.Students.Include(x => x.Schools).FirstOrDefault(StudentSpec.ById(request.UserId));
                if (student != null)
                    return student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId)) ;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<ChangeEmailCommand, Result>
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

            public override async Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserById(request.UserId);
                user.UserName = request.Email;
                user.Email = request.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Errors.Any())
                    throw new CanNotChangeUserEmailException(result.Errors);

                await _notificationService.Send(new AdministratorChangedEmailNotification(user.Email, _siteUrls.Login()));

                return new Result();
            }
        }
    }
}
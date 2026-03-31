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
    public class ResetPasswordCommand : ICommand<ResetPasswordCommand.Result>
    {
        public Guid UserId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ResetPasswordCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).NotEmpty().ExistsInTable(Db.Users);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ResetPasswordCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(ResetPasswordCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var student = _db.Students.Include(x => x.Schools).FirstOrDefault(StudentSpec.ById(request.UserId));
                if (student != null)
	                return student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId));

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<ResetPasswordCommand, Result>
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

            public override async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserById(request.UserId);

                if (string.IsNullOrEmpty(user.Email))
                    throw new UserHasNoEmailException();

                var studentOrTeacher = await Db.Users
                                               .Include(_ => _.Student)
                                               .Include(_ => _.Teacher)
                                               .SingleOrDefaultAsync(UserSpec.ById(user.Id), cancellationToken);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _notificationService.Send(new AdministratorResetPasswordNotification(user.Email, studentOrTeacher.GetFullName(), _siteUrls.SetPassword(user.UserName, token)));

                return new Result();
            }
        }
    }
}

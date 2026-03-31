using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.UserGuide.SendQuestion
{
    [PublicAPI]
    public class SendQuestionCommand : ICommand<SendQuestionCommand.Result>
    {
        public string Text { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<SendQuestionCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Text).NotEmpty().NotNull();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SendQuestionCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(SendQuestionCommand request, IUserSecurityInfo user) => user.IsTeacher() || user.IsStudent() || user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<SendQuestionCommand, Result>
        {
            private readonly ISessionContext _sessionContext;
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;

            public Handler(IDb db, ISessionContext sessionContext, INotificationService notificationService, ISiteUrls siteUrls) : base(db)
            {
                _notificationService = notificationService;
                _siteUrls = siteUrls;
                _sessionContext = sessionContext;
            }

            public override async Task<Result> Handle(SendQuestionCommand request, CancellationToken cancellationToken)
            {
                var userId = _sessionContext.GetUserId();
                var user = await Db.Users.Where(_ => _.Id == userId).SingleAsync(cancellationToken);

                if (string.IsNullOrEmpty(user.Email))
                    throw new UserHasNoEmailException();

                var studentOrTeacher = await Db.Users
                                               .Include(_ => _.Student)
                                               .Include(_ => _.Teacher)
                                               .SingleOrDefaultAsync(UserSpec.ById(user.Id), cancellationToken);
                
                await _notificationService.Send(new QuestionNotification(user.Email, studentOrTeacher.GetFullName(), request.Text));

                return new Result();
            }
        }
    }
}

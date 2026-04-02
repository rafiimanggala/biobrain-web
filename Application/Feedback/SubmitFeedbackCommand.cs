using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Feedback
{
    [PublicAPI]
    public class SubmitFeedbackCommand : ICommand<SubmitFeedbackCommand.Result>
    {
        public int Rating { get; init; }
        public string FeedbackText { get; init; } = "";

        [PublicAPI]
        public class Result
        {
        }

        internal class Validator : ValidatorBase<SubmitFeedbackCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Rating).InclusiveBetween(1, 5);
                RuleFor(_ => _.FeedbackText).MaximumLength(500);
            }
        }

        internal class Handler : CommandHandlerBase<SubmitFeedbackCommand, Result>
        {
            private readonly INotificationService _notificationService;
            private readonly ISessionContext _sessionContext;

            public Handler(IDb db, INotificationService notificationService, ISessionContext sessionContext) : base(db)
            {
                _notificationService = notificationService;
                _sessionContext = sessionContext;
            }

            public override async Task<Result> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
            {
                var user = await Db.Users.AsNoTracking()
                    .Include(_ => _.Teacher)
                    .Include(_ => _.Student)
                    .FirstOrDefaultAsync(UserSpec.ById(_sessionContext.GetUserId()), cancellationToken);

                var userName = user != null ? user.GetFullName() : "Unknown User";

                await _notificationService.Send(new FeedbackNotification(request.Rating, request.FeedbackText, userName));

                return new Result();
            }
        }
    }
}

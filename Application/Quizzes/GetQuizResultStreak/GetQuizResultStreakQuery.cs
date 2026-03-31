using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using JetBrains.Annotations;

namespace Biobrain.Application.Quizzes.GetQuizResultStreak
{
    [PublicAPI]
    public class GetQuizResultStreakCommand : ICommand<GetQuizResultStreakCommand.Result>
    {
	    public Guid CourseId { get; set; }
	    public DateTime LocalDate { get; set; }

        [PublicAPI]
        public class Result
        {
	        public int Streak { get; set; }
	        public double DaysCount { get; set; }
        }


        internal class Validator : ValidatorBase<GetQuizResultStreakCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizResultStreakCommand>
        {
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService)
            {
            }

            protected override bool CanExecute(GetQuizResultStreakCommand request, IUserSecurityInfo user) => user.IsStudent();
        }


        internal class Handler : CommandHandlerBase<GetQuizResultStreakCommand, Result>
        {
	        private readonly ISessionContext _sessionContext;
	        private readonly IQuizStreakService _quizStreakService;

            public Handler(IDb db, ISessionContext sessionContext, IQuizStreakService quizStreakService) : base(db)
            {
                this._sessionContext = sessionContext;
                _quizStreakService = quizStreakService;
            }

            public override async Task<Result> Handle(GetQuizResultStreakCommand request,
	            CancellationToken cancellationToken)
            {
	            var userId = _sessionContext.GetUserId();
	            var streak = await _quizStreakService.GetStreak(userId, request.CourseId, request.LocalDate);
                return streak == null
                    ? new Result { DaysCount = 0, Streak = 0 }
                    : new Result { DaysCount = streak.DaysCount, Streak = streak.Streak };
            }
        }
    }
}

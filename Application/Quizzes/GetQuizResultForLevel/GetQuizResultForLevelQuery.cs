using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetQuizResultForLevel
{
    [PublicAPI]
    public class GetQuizResultForLevelQuery : ICommand<GetQuizResultForLevelQuery.Result>
    {
	    public Guid CourseId { get; set; }
        public List<Guid> LevelIds { get; set; }

	    [PublicAPI]
        public class Result
        {
	        public List<QuizResult> QuizResults { get; set; } = new();
        }

        public record QuizResult(Guid QuizResultId, Guid QuizId, Guid CourseId, Guid NodeId, Guid? ParentNodeId, double Score, DateTime Date);


        internal class Validator : ValidatorBase<GetQuizResultForLevelQuery>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizResultForLevelQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService)
            {
                _db = db;
            }

            protected override bool CanExecute(GetQuizResultForLevelQuery request, IUserSecurityInfo user)
            {
	            return user.IsStudent();
            }
        }


        internal class Handler : CommandHandlerBase<GetQuizResultForLevelQuery, Result>
        {
	        private readonly ISessionContext _sessionContext;

            public Handler(IDb db, ISessionContext sessionContext) : base(db)
            {
                this._sessionContext = sessionContext;
            }

            public override async Task<Result> Handle(GetQuizResultForLevelQuery request,
	            CancellationToken cancellationToken)
            {
	            var userId = _sessionContext.GetUserId();
                var quizAssignments = new List<QuizStudentAssignmentEntity>();
                foreach (var requestLevelId in request.LevelIds)
                {
                    var quizAssignment = await Db.QuizStudentAssignments
                        .Include(_ => _.QuizAssignment)
                        .ThenInclude(x => x.Quiz)
                        .ThenInclude(x => x.ContentTreeNode)
                        .Include(_ => _.Result)
                        .ThenInclude(x => x.Questions)
                        .Where(_ => _.AssignedToUserId == userId &&
                                    _.QuizAssignment.Quiz.ContentTreeNode.CourseId == request.CourseId &&
                                    _.Result != null && _.Result.CompletedAt != null)
                        .Where(_ => requestLevelId == _.QuizAssignment.Quiz.ContentTreeId)
                        .OrderByDescending(x => x.Result.CompletedAt)
                        .SingleOrDefaultAsync(cancellationToken);

                    if(quizAssignment != null) quizAssignments.Add(quizAssignment);
                }

                return new Result
                {
                    QuizResults = quizAssignments.Where(x => x.Result.CompletedAt != null).Select(x =>
                        {
                            return new QuizResult(
                                x.QuizStudentAssignmentId,
                                x.QuizAssignment.QuizId,
                                x.QuizAssignment.Quiz.ContentTreeNode.CourseId,
                                x.QuizAssignment.Quiz.ContentTreeId, 
                                x.QuizAssignment.Quiz.ContentTreeNode.ParentId,
                                x.Result.Questions.Any() ? x.Result.Questions.Count(y => y.IsCorrect) : 0,
                                x.Result.CompletedAt ?? DateTime.MinValue);
                        }
                    ).ToList()
                };
            }
        }
    }
}

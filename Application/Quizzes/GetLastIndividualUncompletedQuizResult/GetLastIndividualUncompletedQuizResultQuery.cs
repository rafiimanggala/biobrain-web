using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetLastIndividualUncompletedQuizResult
{
    [PublicAPI]
    public class GetLastIndividualUncompletedQuizResultQuery : ICommand<GetLastIndividualUncompletedQuizResultQuery.Result>
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid? QuizResultId { get; set; }
        }


        internal class Validator : ValidatorBase<GetLastIndividualUncompletedQuizResultQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetLastIndividualUncompletedQuizResultQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetLastIndividualUncompletedQuizResultQuery request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<GetLastIndividualUncompletedQuizResultQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetLastIndividualUncompletedQuizResultQuery request, CancellationToken cancellationToken)
            {
                var quizResultEntity = await Db.QuizStudentAssignments
                                               .Include(_ => _.QuizAssignment)
                                               .Include(_ => _.Result)
                                               .Where(_ => _.QuizAssignment.AssignedByTeacherId == null)
                                               .Where(_ => _.QuizAssignment.QuizId == request.QuizId)
                                               .Where(_ => _.AssignedToUserId == request.UserId)
                                               .Where(_ => _.Result == null || _.Result.CompletedAt == null)
                                               .OrderBy(_ => _.CreatedAt)
                                               .FirstOrDefaultAsync(cancellationToken);

                return new Result
                       {
                           QuizResultId = quizResultEntity?.QuizStudentAssignmentId
                       };
            }
        }
    }
}

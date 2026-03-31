using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetQuiz
{
    [PublicAPI]
    public sealed class GetQuizByNodeIdQuery : IQuery<List<GetQuizByNodeIdQuery.Result>>
    {
        public Guid NodeId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid QuestionId { get; set; }
            public string Header { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetQuizByNodeIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetQuizByNodeIdQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetQuizByNodeIdQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetQuizByNodeIdQuery request, CancellationToken cancellationToken)
            {
	            var model = await Db.Quizzes.AsNoTracking()
                    .Where(x => x.ContentTreeId == request.NodeId)
		            .Include(x => x.QuizQuestions)
		            .ThenInclude(x => x.Question)
                    .FirstOrDefaultAsync(cancellationToken);

	            return model?.QuizQuestions.OrderBy(x => x.Order).Select(x => new Result
	            {
		            QuestionId = x.QuestionId,
		            Header = x.Question.Header
	            }).ToList() ?? new List<Result>();
            }
        }
    }
}
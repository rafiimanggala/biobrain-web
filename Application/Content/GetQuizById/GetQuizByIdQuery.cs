using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Projections;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetQuizById
{
    [PublicAPI]
    public sealed class GetQuizByIdQuery : IQuery<ContentData.Quiz>
    {
        public Guid QuizId { get; init; }

        internal sealed class PermissionCheck : PermissionCheckBase<GetQuizByIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetQuizByIdQuery request, IUserSecurityInfo user) => true;
        }

        internal sealed class Handler : QueryHandlerBase<GetQuizByIdQuery, ContentData.Quiz>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<ContentData.Quiz> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes
                    .AsNoTracking()
                    .Where(q => q.QuizId == request.QuizId)
                    .PrepareToMapToQuizContentData()
                    .Select(QuizProjection.ToQuizContentData())
                    .FirstOrDefaultAsync(cancellationToken);

                return quiz;
            }
        }
    }
}

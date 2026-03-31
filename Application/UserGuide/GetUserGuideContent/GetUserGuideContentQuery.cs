using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.UserGuide.GetUserGuideContent
{
    [PublicAPI]
    public sealed class GetUserGuideContentQuery : IQuery<GetUserGuideContentQuery.Result>
    {
        public Guid NodeId { get; set; }

        [PublicAPI]
        public record Result
        {
	        public Guid? ArticleId { get; set; }
	        public Guid NodeId { get; set; }
	        public string HtmlText { get; set; }
	        public string VideoUrl { get; set; }
        }


        internal sealed class Validator : ValidatorBase<GetUserGuideContentQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NodeId).ExistsInTable(db.UserGuideContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetUserGuideContentQuery>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(GetUserGuideContentQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin() || user.IsTeacher() || user.IsStudent())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetUserGuideContentQuery, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetUserGuideContentQuery request, CancellationToken cancellationToken)
            {
                var node = await Db.UserGuideContentTree.AsNoTracking()
                    .Include(_ => _.Article)
                    .Where(_ => _.NodeId == request.NodeId)
                    .SingleAsync(cancellationToken);

                return new Result
                {
                    ArticleId = node.Article?.UserGuideArticleId,
                    NodeId = request.NodeId,
                    HtmlText = node.Article?.HtmlText ?? "",
                    VideoUrl = node.Article?.VideoLink ?? ""
                };
            }
        }
    }
}

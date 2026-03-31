using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.UserGuide;
using JetBrains.Annotations;

namespace Biobrain.Application.UserGuide.SaveUserGuideContent
{
    [PublicAPI]
    public sealed class SaveUserGuideContentCommand : IQuery<SaveUserGuideContentCommand.Result>
    {
        public Guid? ArticleId { get; set; }
        public Guid NodeId { get; set; }
        public string HtmlText { get; set; }
        public string VideoUrl { get; set; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<SaveUserGuideContentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NodeId).ExistsInTable(db.UserGuideContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<SaveUserGuideContentCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(SaveUserGuideContentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<SaveUserGuideContentCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(SaveUserGuideContentCommand request, CancellationToken cancellationToken)
            {
                if (request.ArticleId == null)
                {
                    await Db.UserGuideArticles.AddAsync(new UserGuideArticleEntity
                    {
                        UserGuideContentTreeId = request.NodeId,
                        HtmlText = request.HtmlText,
                        VideoLink = request.VideoUrl,

                    }, cancellationToken);
                }
                else
                {
                    var article = await Db.UserGuideArticles.Where(_ => _.UserGuideArticleId == request.ArticleId)
                        .GetSingleAsync(cancellationToken);

                    article.UserGuideContentTreeId = request.NodeId;
                    article.HtmlText = request.HtmlText;
                    article.VideoLink = request.VideoUrl;
                }

                await Db.SaveChangesAsync(cancellationToken);
				return new Result();
            }
        }
    }
}

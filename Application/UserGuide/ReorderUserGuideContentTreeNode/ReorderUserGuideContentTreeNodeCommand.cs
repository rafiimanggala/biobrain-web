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

namespace Biobrain.Application.UserGuide.ReorderUserGuideContentTreeNode
{
    [PublicAPI]
    public sealed class ReorderUserGuideContentTreeNodeCommand : IQuery<ReorderUserGuideContentTreeNodeCommand.Result>
    {
        public Guid NodeId { get; init; }
        public int NewOrder { get; init; }


        [PublicAPI]
        public record Result;


        internal sealed class Validator : ValidatorBase<ReorderUserGuideContentTreeNodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NodeId).ExistsInTable(Db.UserGuideContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<ReorderUserGuideContentTreeNodeCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(ReorderUserGuideContentTreeNodeCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<ReorderUserGuideContentTreeNodeCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(ReorderUserGuideContentTreeNodeCommand request, CancellationToken cancellationToken)
            {
	            var node = await Db.UserGuideContentTree.AsNoTracking().SingleAsync(_ => _.NodeId == request.NodeId, cancellationToken);
                var siblings = (await Db.UserGuideContentTree.Where(_ => _.ParentId == node.ParentId).ToListAsync(cancellationToken)).OrderBy(_ => _.Order);
                
                bool seen = false;
                foreach (var sibling in siblings)
                {
                        if (sibling.NodeId == node.NodeId)
                        {
                            sibling.Order = request.NewOrder;
                            seen = true;
                        }
                        else if (seen)
                        {
                            if (sibling.Order <= request.NewOrder)
                            {
                                sibling.Order--; // move it left
                            }
                        }
                        else
                        {
                            if (sibling.Order >= request.NewOrder)
                            {
                                sibling.Order++; // move it right
                            }
                        }
                }

                await Db.SaveChangesAsync(cancellationToken);

				return new Result();
            }
        }
    }
}

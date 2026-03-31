using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.UserGuide.DeleteUserGuideNode
{
    [PublicAPI]
    public sealed class DeleteUserGuideNodeCommand : IQuery<DeleteUserGuideNodeCommand.Result>
    {
        public Guid NodeId { get; init; }


        [PublicAPI]
        public record Result;


        internal sealed class Validator : ValidatorBase<DeleteUserGuideNodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NodeId).ExistsInTable(Db.UserGuideContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<DeleteUserGuideNodeCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(DeleteUserGuideNodeCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<DeleteUserGuideNodeCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteUserGuideNodeCommand request, CancellationToken cancellationToken)
            {
	            var node = await Db.UserGuideContentTree.SingleAsync(_ => _.NodeId == request.NodeId, cancellationToken);
				Db.UserGuideContentTree.Remove(node);
	            await Db.SaveChangesAsync(cancellationToken);

				return new Result();
            }
        }
    }
}

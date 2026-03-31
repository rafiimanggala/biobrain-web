using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.DeleteContentTreeNode
{
    [PublicAPI]
    public class DeleteContentTreeNodeCommand : ICommand<DeleteContentTreeNodeCommand.Result>
    {
        public Guid NodeId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteContentTreeNodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NodeId).ExistsInTable(Db.ContentTree);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteContentTreeNodeCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(DeleteContentTreeNodeCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<DeleteContentTreeNodeCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteContentTreeNodeCommand request, CancellationToken cancellationToken)
            {
                var node = await Db.ContentTree.FindAsync(request.NodeId);
                await RemoveRecursively(node);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }

            private async Task RemoveRecursively(ContentTreeEntity node)
            {
	            var children = await Db.ContentTree.Where(x => x.ParentId == node.NodeId).ToListAsync();
	            if (children.Any())
	            {
		            foreach (var child in children)
		            {
			            await RemoveRecursively(child);
		            }
	            }

	            Db.ContentTree.Remove(node);
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.SwitchOrderForNodes
{
    [PublicAPI]
    public sealed class SwitchOrderForNodesCommand : IQuery<SwitchOrderForNodesCommand.Result>
    {
	    public Guid Entity1Id { get; set; }
	    public Guid Entity2Id { get; set; }


        [PublicAPI]
        public record Result
        {
            public Guid Entity1Id { get; set; }
            public long Order1 { get; set; }

            public Guid Entity2Id { get; set; }
            public long Order2 { get; set; }
        }

        internal class Validator : ValidatorBase<SwitchOrderForNodesCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.Entity1Id).ExistsInTable(Db.ContentTree);
		        RuleFor(_ => _.Entity2Id).ExistsInTable(Db.ContentTree);
		        RuleFor(_ => _.Entity1Id).MustAsync(async (command, _, _) =>
			        (await Db.ContentTree.FindAsync(command.Entity1Id)).ParentId ==
			        (await Db.ContentTree.FindAsync(command.Entity2Id)).ParentId);
	        }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<SwitchOrderForNodesCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(SwitchOrderForNodesCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<SwitchOrderForNodesCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(SwitchOrderForNodesCommand request, CancellationToken cancellationToken)
            {
                // Get entities
	            var node1 = await Db.ContentTree.AsNoTracking().FirstAsync(x => x.NodeId == request.Entity1Id, cancellationToken);
	            var node2 = await Db.ContentTree.AsNoTracking().FirstAsync(x => x.NodeId == request.Entity2Id, cancellationToken);
	            var siblings = await Db.ContentTree.Where(x => x.ParentId == node1.ParentId)
		            .ToListAsync(cancellationToken);

                // Switch order
	            var entity1 = siblings.First(x => x.NodeId == node1.NodeId);
	            entity1.Order = node2.Order;

                var entity2 = siblings.First(x => x.NodeId == node2.NodeId);
                entity2.Order = node1.Order;

                var i = 0;
                //Update order to avoid order issues
                foreach (var node in siblings.OrderBy(x => x.Order))
                {
	                node.Order = i;
	                i++;
                }

                await Db.SaveChangesAsync(cancellationToken);
                return new Result{ Entity1Id = entity1.NodeId, Entity2Id = entity2.NodeId, Order1 = entity1.Order, Order2 = entity2.Order};
            }
        }
    }
}
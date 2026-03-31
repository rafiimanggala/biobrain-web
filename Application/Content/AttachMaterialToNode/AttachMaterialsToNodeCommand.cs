using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Material;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.AttachMaterialToNode
{
    [PublicAPI]
    public sealed class AttachMaterialsToNodeCommand : IQuery<AttachMaterialsToNodeCommand.Result>
    {
	    public Guid NodeId { get; set; }
	    public List<KeyValue> MaterialIds { get; set; }
	    public bool IsReplaceMode { get; set; }


	    public record KeyValue
	    {
            public int Key { get; set; }
            public Guid Value { get; set; }
	    };

        [PublicAPI]
        public record Result;

        internal class Validator : ValidatorBase<AttachMaterialsToNodeCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.NodeId).ExistsInTable(Db.ContentTree);
		        //RuleForEach(_ => _.MaterialIds.Select(x => x.Value)).ExistsInTable(Db.Materials);
                RuleFor(_ => _.NodeId)
                   .MustAsync(async (command, _, _) => (
                                                           await Db.ContentTree
                                                                   .Where(x => x.NodeId == command.NodeId)
                                                                   .Include(x => x.ContentTreeMeta)
                                                                   .FirstAsync())
                                                      .ContentTreeMeta.CouldAddContent)
                   .WithMessage("Can't attach materials to this node.");
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<AttachMaterialsToNodeCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(AttachMaterialsToNodeCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<AttachMaterialsToNodeCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(AttachMaterialsToNodeCommand request,
	            CancellationToken cancellationToken)
            {
	            // Get existing page or create new
	            var page =
		            (await Db.Pages.AsNoTracking()
			            .Where(x => x.ContentTreeId == request.NodeId)
			            .FirstOrDefaultAsync(cancellationToken))

		            ?? (await Db.Pages.AddAsync(new PageEntity
		            {
			            ContentTreeId = request.NodeId,
		            }, cancellationToken)).Entity;

	            var links = await Db.PageMaterials
		            .Where(x => x.PageId == page.PageId)
		            .ToListAsync(cancellationToken);

	            // Update order for existing
	            links.Where(x => request.MaterialIds.Any(y => y.Value == x.MaterialId)).ToList().ForEach(x =>
	            {
		            var order = request.MaterialIds.First(m => m.Value == x.MaterialId).Key;
		            x.Order = order;
	            });

	            // Delete no needed
	            var toDelete = links.Where(x => request.MaterialIds.All(y => y.Value != x.MaterialId));
	            Db.PageMaterials.RemoveRange(toDelete);

				// Add new
				await Db.PageMaterials.AddRangeAsync(request.MaterialIds
		            .Where(x => links.All(y => y.MaterialId != x.Value))
		            .Select(x => new PageMaterialEntity
		            {
			            PageId = page.PageId,
			            MaterialId = x.Value,
			            Order = x.Key
		            }), cancellationToken);

	            await Db.SaveChangesAsync(cancellationToken);

	            return new Result();
            }
        }
    }
}
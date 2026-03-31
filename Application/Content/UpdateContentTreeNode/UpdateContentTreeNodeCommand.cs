using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.UpdateContentTreeNode
{
    [PublicAPI]
    public sealed class UpdateContentTreeNodeCommand : IQuery<UpdateContentTreeNodeCommand.Result>
    {
	    public Guid EntityId { get; set; }
	    public string Header { get; set; }
        public bool IsAvailableInDemo { get; set; }


        [PublicAPI]
        public record Result
        {
            public Guid EntityId { get; set; }
            public Guid? ParentId { get; set; }
            public long Depth { get; set; }
            public string Header { get; set; }
            public long Order { get; set; }
            public bool IsCanAddChildren { get; set; }
            public bool IsCanAttachContent { get; set; }
            public bool IsAvailableInDemo { get; set; }
        }

        internal class Validator : ValidatorBase<UpdateContentTreeNodeCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.Header).NotEmpty();
		        RuleFor(_ => _.EntityId).ExistsInTable(Db.ContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UpdateContentTreeNodeCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(UpdateContentTreeNodeCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<UpdateContentTreeNodeCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(UpdateContentTreeNodeCommand request, CancellationToken cancellationToken)
            {
	            var entity = await Db.ContentTree.Include(x => x.ContentTreeMeta)
		            .FirstAsync(x => x.NodeId == request.EntityId, cancellationToken);
                entity.Name = request.Header;
                entity.IsAvailableInDemo = request.IsAvailableInDemo;

                await Db.SaveChangesAsync(cancellationToken);
                return ResultByContentTreeEntity(entity);
            }

            private Result ResultByContentTreeEntity(ContentTreeEntity model) => new()
            {
                EntityId = model.NodeId,
                ParentId = model.ParentId,
                Depth = model.ContentTreeMeta.Depth,
                Header = model.Name,
                IsCanAddChildren = model.ContentTreeMeta.CouldAddEntry,
                IsCanAttachContent = model.ContentTreeMeta.CouldAddContent,
                Order = model.Order,
                IsAvailableInDemo = model.IsAvailableInDemo,
            };
        }
    }
}
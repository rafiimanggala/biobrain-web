using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using DataAccessLayer.WebAppEntities;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Biobrain.Application.Content.CreateContentTreeNode
{
    [PublicAPI]
    public sealed class CreateContentTreeNodeCommand : IQuery<List<CreateContentTreeNodeCommand.Result>>
    {
	    public Guid CourseId { get; set; }
        public Guid? ParentId { get; set; }
	    public string Header { get; set; }
	    public long Order { get; set; }
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

        internal class Validator : ValidatorBase<CreateContentTreeNodeCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.Header).NotEmpty();
		        RuleFor(_ => _.Order).GreaterThanOrEqualTo(0);
		        RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                // ToDo: Validate null or Exist in table
                //RuleFor(_ => _.ParentId).ExistsInTable(Db.ContentTree);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<CreateContentTreeNodeCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(CreateContentTreeNodeCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<CreateContentTreeNodeCommand, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(CreateContentTreeNodeCommand request, CancellationToken cancellationToken)
            {
	            var parent = await Db.ContentTree.AsNoTracking()
		            .Include(x => x.ContentTreeMeta)
		            .FirstOrDefaultAsync(x => x.NodeId == request.ParentId, cancellationToken);

	            var meta = await Db.ContentTreeMeta.AsNoTracking()
		            .FirstAsync(x => x.Depth == (parent == null ? 0 : (parent.ContentTreeMeta.Depth + 1)) && x.CourseId == request.CourseId,
			            cancellationToken);

                // Increase order for all siblings that has greater order
	            var siblings = await Db.ContentTree
		            .Where(x => x.ParentId == request.ParentId && x.Order >= request.Order)
		            .ToListAsync(cancellationToken);
                siblings.ForEach(x => x.Order++);

                // Create entity
                var entities = new Dictionary<EntityEntry<ContentTreeEntity>, ContentTreeMetaEntity>();
                var entity = Db.ContentTree
                    .Add(new ContentTreeEntity
                    {
                        ContentTreeMetaId = meta.ContentTreeMetaId,
                        CourseId = request.CourseId,
                        Name = request.Header,
                        Order = request.Order,
                        ParentId = request.ParentId,
                        IsAvailableInDemo = request.IsAvailableInDemo
                    });
                entities.Add(entity, meta);

                // Add levels if topic
                if (!meta.CouldAddContent && !meta.CouldAddEntry)
                {
	                for (var i = 0; i < 3; i++)
	                {
		                var levelMeta = await Db.ContentTreeMeta.AsNoTracking()
			                .FirstAsync(x => x.Depth == meta.Depth+1 && x.CourseId == request.CourseId,
				                cancellationToken);

		                entities.Add(Db.ContentTree.Add(new ContentTreeEntity
		                {
			                ContentTreeMetaId = levelMeta.ContentTreeMetaId,
			                CourseId = request.CourseId,
			                Name = $"Level {i+1}",
			                Order = i,
			                ParentId = entity.Entity.NodeId,
                        }), levelMeta);
	                }
                }

                await Db.SaveChangesAsync(cancellationToken);
                return entities.Select(x => ResultByContentTreeEntity(x.Key.Entity, x.Value)).ToList();
            }

            private Result ResultByContentTreeEntity(ContentTreeEntity model, ContentTreeMetaEntity meta) => new()
            {
                EntityId = model.NodeId,
                ParentId = model.ParentId,
                Depth = meta.Depth,
                Header = model.Name,
                IsCanAddChildren = meta.CouldAddEntry,
                IsCanAttachContent = meta.CouldAddContent,
                Order = model.Order,
                IsAvailableInDemo = model.IsAvailableInDemo
            };
        }
    }
}
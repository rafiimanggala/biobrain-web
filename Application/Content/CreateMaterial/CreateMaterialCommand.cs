using System;
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

namespace Biobrain.Application.Content.CreateMaterial
{
    [PublicAPI]
    public sealed class CreateMaterialCommand : ICommand<CreateMaterialCommand.Result>
    {
        public Guid CourseId { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string VideoLink { get; set; }

        // Optional: attach to a content tree node (leaf, must allow content)
        public Guid? NodeId { get; set; }

        [PublicAPI]
        public sealed class Result
        {
            public Guid MaterialId { get; set; }
        }

        internal class Validator : ValidatorBase<CreateMaterialCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.Header).NotEmpty().MaximumLength(500);
                RuleFor(_ => _.Text).NotEmpty();

                When(_ => _.NodeId.HasValue, () =>
                {
                    RuleFor(_ => _.NodeId.Value)
                        .ExistsInTable(Db.ContentTree);

                    RuleFor(_ => _.NodeId.Value)
                        .MustAsync(async (command, nodeId, _) => (
                                await Db.ContentTree
                                    .Where(x => x.NodeId == nodeId)
                                    .Include(x => x.ContentTreeMeta)
                                    .FirstAsync())
                            .ContentTreeMeta.CouldAddContent)
                        .WithMessage("Can't attach materials to this node.");
                });
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<CreateMaterialCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(CreateMaterialCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<CreateMaterialCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
            {
                await using var transaction = await Db.BeginTransactionAsync(cancellationToken);

                var material = new MaterialEntity
                {
                    CourseId = request.CourseId,
                    Header = request.Header?.Trim() ?? "",
                    Text = request.Text ?? "",
                    VideoLink = request.VideoLink?.Trim() ?? ""
                };

                var matEntry = await Db.AddAsync(material, cancellationToken);
                var materialId = ((MaterialEntity)matEntry.Entity).MaterialId;

                if (request.NodeId.HasValue)
                {
                    var page = await Db.Pages
                                    .Where(x => x.ContentTreeId == request.NodeId.Value)
                                    .FirstOrDefaultAsync(cancellationToken)
                                ?? (await Db.Pages.AddAsync(new PageEntity
                                {
                                    ContentTreeId = request.NodeId.Value
                                }, cancellationToken)).Entity;

                    var maxOrder = await Db.PageMaterials
                        .Where(x => x.PageId == page.PageId)
                        .Select(x => (int?)x.Order)
                        .MaxAsync(cancellationToken) ?? -1;

                    await Db.AddAsync(new PageMaterialEntity
                    {
                        PageId = page.PageId,
                        MaterialId = materialId,
                        Order = maxOrder + 1
                    }, cancellationToken);
                }

                await Db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new Result { MaterialId = materialId };
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Material;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Materials.ExcludedMaterials
{
    [PublicAPI]
    public sealed class AddExcludedMaterialCommand : ICommand<AddExcludedMaterialCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public Guid MaterialId { get; init; }
        public bool IsExcluded { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<AddExcludedMaterialCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.MaterialId).ExistsInTable(Db.Materials);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<AddExcludedMaterialCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(AddExcludedMaterialCommand request, IUserSecurityInfo user) => true;
        }


        internal sealed class Handler : CommandHandlerBase<AddExcludedMaterialCommand, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(AddExcludedMaterialCommand request, CancellationToken cancellationToken)
            {
                if (request.IsExcluded)
                {
                    var existing = await Db.ExcludedMaterials
                        .Where(_ => _.SchoolClassId == request.SchoolClassId && _.MaterialId == request.MaterialId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (existing != null) return new Result();

                    await Db.ExcludedMaterials.AddAsync(new ExcludedMaterialEntity
                    {
                        ExcludedMaterialId = Guid.NewGuid(),
                        SchoolClassId = request.SchoolClassId,
                        MaterialId = request.MaterialId,
                        ExcludedAtUtc = DateTime.UtcNow,
                    }, cancellationToken);
                }
                else
                {
                    var entity = await Db.ExcludedMaterials
                        .Where(_ => _.SchoolClassId == request.SchoolClassId && _.MaterialId == request.MaterialId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (entity == null) return new Result();

                    Db.Remove(entity);
                }

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}

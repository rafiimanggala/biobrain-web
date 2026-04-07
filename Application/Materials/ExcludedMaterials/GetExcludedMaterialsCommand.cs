using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Materials.ExcludedMaterials
{
    [PublicAPI]
    public sealed class GetExcludedMaterialsCommand : ICommand<GetExcludedMaterialsCommand.Result>
    {
        public Guid SchoolClassId { get; init; }


        [PublicAPI]
        public record Result
        {
            public ImmutableList<Guid> MaterialIds { get; init; }
        }


        internal sealed class Validator : ValidatorBase<GetExcludedMaterialsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetExcludedMaterialsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetExcludedMaterialsCommand request, IUserSecurityInfo user) => true;
        }


        internal sealed class Handler : CommandHandlerBase<GetExcludedMaterialsCommand, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetExcludedMaterialsCommand request, CancellationToken cancellationToken)
            {
                var excluded = await Db.ExcludedMaterials
                    .Where(_ => _.SchoolClassId == request.SchoolClassId)
                    .Select(_ => _.MaterialId)
                    .ToListAsync(cancellationToken);

                return new Result
                {
                    MaterialIds = excluded.ToImmutableList(),
                };
            }
        }
    }
}

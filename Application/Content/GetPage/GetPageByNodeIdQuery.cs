using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetPage
{
    [PublicAPI]
    public sealed class GetPageByNodeIdQuery : IQuery<List<GetPageByNodeIdQuery.Result>>
    {
        public Guid NodeId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid MaterialId { get; set; }
            public string Header { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetPageByNodeIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetPageByNodeIdQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetPageByNodeIdQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetPageByNodeIdQuery request, CancellationToken cancellationToken)
            {
	            var model = await Db.Pages.AsNoTracking()
                    .Where(x => x.ContentTreeId == request.NodeId)
		            .Include(x => x.PageMaterials)
		            .ThenInclude(x => x.Material)
                    .FirstOrDefaultAsync(cancellationToken);

	            return model?.PageMaterials.OrderBy(x => x.Order).Select(x => new Result
	            {
		            MaterialId = x.MaterialId,
		            Header = x.Material.Header
	            }).ToList() ?? new List<Result>();
            }
        }
    }
}
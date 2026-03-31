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


namespace Biobrain.Application.Schools.GetSchoolListItems
{
    [PublicAPI]
    public sealed class GetSchoolListItemsQuery : IQuery<List<GetSchoolListItemsQuery.Result>>
    {
        [PublicAPI]
        public record Result
        {
            public Guid SchoolId { get; init; }
            public string Name { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolListItemsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolListItemsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }

        internal sealed class Handler : QueryHandlerBase<GetSchoolListItemsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }
            public override Task<List<Result>> Handle(GetSchoolListItemsQuery request, CancellationToken cancellationToken)
            {
                return Db.Schools
                         .Select(_ => new Result
                                      {
                                          SchoolId = _.SchoolId,
                                          Name = _.Name
                                      })
                         .AsNoTracking()
                         .OrderBy(x => x.Name)
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
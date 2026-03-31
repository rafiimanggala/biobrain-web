using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.SchoolClasses.GetSchoolClassListItems
{
    [PublicAPI]
    public sealed class GetSchoolClassListItemsQuery : IQuery<List<GetSchoolClassListItemsQuery.Result>>
    {
        public Guid SchoolId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid SchoolClassId { get; init; }
            public string Name { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolClassListItemsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolClassListItemsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.HasAccessToSchool(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolClassListItemsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetSchoolClassListItemsQuery request, CancellationToken cancellationToken)
            {
                return Db.SchoolClasses
                         .Where(SchoolClassSpec.ForSchool(request.SchoolId))
                         .Select(_ => new Result
                                      {
                                          SchoolClassId = _.SchoolClassId,
                                          Name = _.Name
                                      })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
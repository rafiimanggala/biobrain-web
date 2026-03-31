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


namespace Biobrain.Application.Teachers.GetTeacherListItems
{
    [PublicAPI]
    public sealed class GetTeacherListItemsQuery : IQuery<List<GetTeacherListItemsQuery.Result>>
    {
        public Guid SchoolId { get; init; }
        public string SearchText { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid TeacherId { get; init; }
            public string FullName { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetTeacherListItemsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetTeacherListItemsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.HasAccessToSchool(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetTeacherListItemsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetTeacherListItemsQuery request, CancellationToken cancellationToken)
            {
                return Db.Teachers
	                .Include(x => x.Schools)
                         .Where(TeacherSpec.ForSchool(request.SchoolId))
                         .Where(TeacherSpec.WithSimilarName(request.SearchText))
                         .Select(_ => new Result
                                      {
                                          TeacherId = _.TeacherId,
                                          FullName = $"{_.FirstName} {_.LastName}"
                                      })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Students.GetStudentsListItems
{
    [PublicAPI]
    public sealed class GetStudentListItemsQuery : IQuery<List<GetStudentListItemsQuery.Result>>
    {
        public Guid SchoolId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid StudentId { get; init; }
            public string FullName { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetStudentListItemsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetStudentListItemsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.HasAccessToSchool(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetStudentListItemsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetStudentListItemsQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
                         .Where(StudentSpec.ForSchool(request.SchoolId))
                         .Select(_ => new Result
                                      {
                                          StudentId = _.StudentId,
                                          FullName = $"{_.FirstName} {_.LastName}",
                                      })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
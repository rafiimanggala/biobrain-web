using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Schools.GetSchools
{
    [PublicAPI]
    public sealed class GetSchoolListQuery : IQuery<List<GetSchoolListQuery.Result>>
    {
	    public List<Guid> Ids { get; set; }

	    [PublicAPI]
        public record Result
        {
            public Guid SchoolId { get; init; }
            public string Name { get; init; }
            public Constant.SchoolStatus Status { get; init; }
            public int TeachersLicensesNumber { get; init; }
            public int StudentLicensesNumber { get; init; }
            public int TeachersCount { get; init; }
            public int StudentsCount { get; init; }
            public DateTime StartDateUtc { get; set; }
            public DateTime? EndDateUtc { get; set; }
		}


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || request.Ids.All(user.IsSchoolAdmin);
        }

        internal sealed class Handler : QueryHandlerBase<GetSchoolListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetSchoolListQuery request, CancellationToken cancellationToken)
            {
                if(request.Ids == null || !request.Ids.Any())
	                return Db.Schools
	                         .Include(_ => _.Teachers)
	                         .Include(_ => _.Students)
	                         .Select(_ => new Result
	                                      {
	                                          SchoolId = _.SchoolId,
	                                          Name = _.Name,
	                                          TeachersLicensesNumber = _.TeachersLicensesNumber,
	                                          StudentLicensesNumber = _.StudentsLicensesNumber,
	                                          TeachersCount = _.Teachers.Count(),
	                                          StudentsCount = _.Students.Count(),
		                                      Status = _.Status,
                                              StartDateUtc = _.CreatedAt,
								 EndDateUtc = _.EndDateUtc
							 })
	                         .OrderBy(x => x.Name)
	                         .AsNoTracking()
	                         .ToListAsync(cancellationToken);
				else
					return Db.Schools
						.Where(SchoolSpec.ByIds(request.Ids))
						.Include(_ => _.Teachers)
						.Include(_ => _.Students)
						.Select(_ => new Result
						{
							SchoolId = _.SchoolId,
							Name = _.Name,
							TeachersLicensesNumber = _.TeachersLicensesNumber,
							StudentLicensesNumber = _.StudentsLicensesNumber,
							TeachersCount = _.Teachers.Count(),
							StudentsCount = _.Students.Count(),
							Status = _.Status,
                            StartDateUtc = _.CreatedAt,
							EndDateUtc = _.EndDateUtc
						})
						.OrderBy(x => x.Name)
						.AsNoTracking()
						.ToListAsync(cancellationToken);
			}
        }
    }
}

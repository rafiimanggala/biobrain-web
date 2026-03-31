using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Schools.GetSchoolLicenseInfo
{
    [PublicAPI]
    public sealed class GetSchoolLicenseInfoQuery : IQuery<GetSchoolLicenseInfoQuery.Result>
    {
        public Guid SchoolId { get; init; }

        [PublicAPI]
        public record Result
        {
            public Guid SchoolId { get; init; }
            public int TeachersLicensesNumber { get; init; }
            public int ActualTeachersCount { get; init; }
            public int StudentLicensesNumber { get; init; }
            public int ActualStudentsCount { get; init; }
        }


        internal class Validator : ValidatorBase<GetSchoolLicenseInfoQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolLicenseInfoQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolLicenseInfoQuery request, IUserSecurityInfo user) => user.IsSchoolAdmin(request.SchoolId) || user.IsApplicationAdmin();
        }

        internal sealed class Handler : QueryHandlerBase<GetSchoolLicenseInfoQuery, Result>
        {
            public Handler(IDb db) : base(db) { }
            public override Task<Result> Handle(GetSchoolLicenseInfoQuery request, CancellationToken cancellationToken)
            {
                return Db.Schools
                         .Where(Specifications.SchoolSpec.ById(request.SchoolId))
                         .Include(_ => _.Teachers)
                         .Include(_ => _.Students)
                         .Select(_ => new Result
                                      {
                                          SchoolId = _.SchoolId,
                                          TeachersLicensesNumber = _.TeachersLicensesNumber,
                                          StudentLicensesNumber = _.StudentsLicensesNumber,
                                          ActualTeachersCount = _.Teachers.Count,
                                          ActualStudentsCount = _.Students.Count
                                      })
                         .AsNoTracking()
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}

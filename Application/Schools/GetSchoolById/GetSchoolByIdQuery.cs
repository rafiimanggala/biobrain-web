using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Schools.GetSchoolById
{
    [PublicAPI]
    public sealed class GetSchoolByIdQuery : IQuery<GetSchoolByIdQuery.Result>
    {
        public Guid SchoolId { get; init; }

        [PublicAPI]
        public record Result
        {
            public Guid SchoolId { get; init; }
            public string Name { get; init; }
            public int TeachersLicensesNumber { get; init; }
            public int StudentLicensesNumber { get; init; }
            public bool UseAccessCodes { get; init; }
            public Constant.SchoolStatus Status { get; init; }
            public List<Guid> Admins { get; init; }
            public List<Guid> Courses { get; init; }
            public DateTime StartDateUtc { get; set; }
            public DateTime? EndDateUtc { get; set; }
        }


        internal class Validator : ValidatorBase<GetSchoolByIdQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolByIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolByIdQuery request, IUserSecurityInfo user) => user.HasAccessToSchool(request.SchoolId);
        }

        internal sealed class Handler : QueryHandlerBase<GetSchoolByIdQuery, Result>
        {
            public Handler(IDb db) : base(db) { }
            public override Task<Result> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
            {
                return Db.Schools
                         .Where(Specifications.SchoolSpec.ById(request.SchoolId))
                         .Include(_ => _.SchoolAdmins)
                         .ThenInclude(_ => _.Teacher)
                         .Include(_ => _.Courses)
                         .Select(_ => new Result
                                      {
                                          SchoolId = _.SchoolId,
                                          Name = _.Name,
                                          TeachersLicensesNumber = _.TeachersLicensesNumber,
                                          StudentLicensesNumber = _.StudentsLicensesNumber,
                                          Admins = _.SchoolAdmins.Select(_ => _.TeacherId).ToList(),
                                          Courses = _.Courses.Select(_ => _.CourseId).ToList(),
                                          Status = _.Status,
                                          UseAccessCodes = _.UseAccessCodes,
                                          StartDateUtc = _.CreatedAt,
                                          EndDateUtc = _.EndDateUtc
                         })
                         .AsNoTracking()
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}

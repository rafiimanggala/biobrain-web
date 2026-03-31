using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.School;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.GetSchoolClassByCourseIdQuery;

[PublicAPI]
public sealed class GetTeacherSchoolClassesByCourseIdQuery : IQuery<List<GetTeacherSchoolClassesByCourseIdQuery.Result>>
{
    public Guid CourseId { get; init; }

    public Guid SchoolId { get; init; }


    [PublicAPI]
    public record Result
    {
        public Guid SchoolClassId { get; init; }
        public Guid SchoolId { get; init; }
        public Guid CourseId { get; init; }
        public int Year { get; init; }
        public string Name { get; init; }
        public string AutoJoinClassCode { get; init; }
    }


    internal sealed class PermissionCheck : PermissionCheckBase<GetTeacherSchoolClassesByCourseIdQuery>
    {
        public PermissionCheck(ISecurityService securityService, IDb db)
            : base(securityService)
            => _db = db;

        protected override bool CanExecute(GetTeacherSchoolClassesByCourseIdQuery request, IUserSecurityInfo user)
        {
            if (user.IsApplicationAdmin())
                return true;

            SchoolEntity school = _db.Schools.GetSingle(SchoolSpec.ById(request.SchoolId));
            if (user.IsSchoolAdmin(school.SchoolId))
                return true;

            // TODO: perhaps teachers assigned (related to) CLASS should be able to execute this query
            if (user.IsSchoolTeacher(school.SchoolId))
                return true;

            return false;
        }

        private readonly IDb _db;
    }


    internal sealed class Handler(IDb db, ISessionContext sessionContext) : QueryHandlerBase<GetTeacherSchoolClassesByCourseIdQuery, List<Result>>(db)
    {
        private ISessionContext SessionContext { get; } = sessionContext;

        public override async Task<List<Result>> Handle(GetTeacherSchoolClassesByCourseIdQuery request, CancellationToken cancellationToken)
        {
            List<Guid> classIds = await Db.Teachers
                                          .Where(TeacherSpec.ById(SessionContext.GetUserId()))
                                          .Include(x => x.Classes)
                                          .ThenInclude(x => x.SchoolClass)
                                          .SelectMany(x => x.Classes)
                                          .Select(x => x.SchoolClassId)
                                          .ToListAsync(cancellationToken);

            return await Db.SchoolClasses
                           .Where(SchoolClassSpec.ByIds(classIds))
                           .Where(SchoolClassSpec.ForCourse(request.CourseId))
                           .Where(SchoolClassSpec.ForSchool(request.SchoolId))
                           .Select(x => new Result
                                        {
                                            SchoolClassId = x.SchoolClassId,
                                            SchoolId = x.SchoolId,
                                            CourseId = x.CourseId,
                                            Year = x.Year,
                                            Name = x.Name,
                                            AutoJoinClassCode = x.AutoJoinClassCode
                                        })
                           .AsNoTracking()
                           .ToListAsync(cancellationToken);
        }
    }
}

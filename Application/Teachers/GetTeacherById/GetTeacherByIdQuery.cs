using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Teachers.GetTeacherById
{
    [PublicAPI]
    public sealed class GetTeacherByIdQuery : IQuery<GetTeacherByIdQuery.Result>
    {
        public Guid TeacherId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid TeacherId { get; init; }
            public string Email { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public Guid SchoolId { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetTeacherByIdQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetTeacherByIdQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                if (user.IsTeacherAccountOwner(request.TeacherId))
                    return true;

                var teacher = _db.Teachers.Include(x => x.Schools).GetSingle(TeacherSpec.ById(request.TeacherId));
                return teacher.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId));
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetTeacherByIdQuery, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<Result> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
            {
                return Db.Teachers
                         .Where(TeacherSpec.ById(request.TeacherId))
                         .Include(_ => _.User)
                         .Select(_ => new Result
                                      {
                                          TeacherId = _.TeacherId,
                                          Email = _.User.Email,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          //SchoolId = _.SchoolId
                                      })
                         .AsNoTracking()
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}

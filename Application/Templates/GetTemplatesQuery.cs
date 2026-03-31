using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Templates
{
    [PublicAPI]
    public sealed class GetTemplatesQuery : IQuery<List<GetTemplatesQuery.Result>>
    {

        [PublicAPI]
        public record Result
        {
            public Guid TemplateId { get; init; }
            public string Template { get; init; }
            public int TemplateType { get; init; }
            public List<Course> Courses { get; init; }
        }

        public record Course
        {
            public Guid CourseId { get; init; }
            public string Name { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetTemplatesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }
            protected override bool CanExecute(GetTemplatesQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetTemplatesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }
            public override async Task<List<Result>> Handle(GetTemplatesQuery request, CancellationToken ct)
            {
                var templates = await Db.Templates.AsNoTracking().ToListAsync(ct);
                var courseTemplates = await Db.CourseTemplates.AsNoTracking()
                    .Include(_ => _.Course).ThenInclude(_ => _.Curriculum)
                    .Include(_ => _.Course).ThenInclude(_ => _.Subject)
                    .ToListAsync(ct);

                return templates.OrderBy(_ => _.Type).Select(_ => new Result
                {
                    TemplateId = _.TemplateId,
                    Template = _.Value,
                    TemplateType = _.Type,
                    Courses = courseTemplates.Where(ct => ct.TemplateId == _.TemplateId)
                        .Select(ct => new Course
                        {
                            CourseId = ct.CourseId,
                            Name = CourseHelper.GetCourseName(ct.Course)
                        }).OrderBy(_ => _.Name).ToList()
                }).ToList();
            }
        }
    }
}

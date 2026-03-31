using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Courses.GetCourses
{
    [PublicAPI]
    public class GetAllCoursesQuery : ICommand<List<GetAllCoursesQuery.Result>>
    {
        [PublicAPI]
        public class Result
        {
            public Guid CourseId { get; set; }
            public string Name { get; set; }
            public int SubjectCode { get; set; }
            public int CurriculumCode { get; set; }
            public int Year { get; set; }
            public string SubHeader { get; init; }
        }


        internal class Handler : CommandHandlerBase<GetAllCoursesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<Result>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
            {
                return await Db.Courses
                    .Include(x => x.Curriculum)
                    .Include(x => x.Subject)
                               .Select(_ => new Result
                               {
                                   CourseId = _.CourseId,
                                   Name = CourseHelper.GetCourseName(_),
                                   CurriculumCode = _.CurriculumCode,
                                   SubjectCode = _.SubjectCode,
                                   Year = _.Year,
                                   SubHeader = _.SubHeader
                               })
                               .ToListAsync(cancellationToken);
            }
        }
    }
}
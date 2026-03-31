using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.ContentTreeService;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetCourseQuizzesList
{
    [PublicAPI]
    public sealed class GetCourseQuizzesListQuery : IQuery<List<GetCourseQuizzesListQuery.Result>>
    {
        public Guid CourseId { get; set; }

	    [PublicAPI]
        public record Result
        {
            public Guid QuizId { get; init; }
            public string Name { get; init; }
        }


        internal class Validator : ValidatorBase<GetCourseQuizzesListQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetCourseQuizzesListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetCourseQuizzesListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetCourseQuizzesListQuery, List<Result>>
        {
            private readonly IContentTreeService _contentTreeService;
            private readonly ITemplateService _templateService;
            private readonly IContentTreePathResolver _contentTreePathResolver;

            public Handler(IDb db, IContentTreeService contentTreeService, ITemplateService templateService, IContentTreePathResolver contentTreePathResolver) : base(db)
            {
                _contentTreeService = contentTreeService;
                _templateService = templateService;
                _contentTreePathResolver = contentTreePathResolver;
            }

            public override async Task<List<Result>> Handle(GetCourseQuizzesListQuery request, CancellationToken cancellationToken)
            {
                var course = await Db.Courses
                    .Where(CourseSpec.ById(request.CourseId))
	                .Include(x => x.Curriculum)
	                .Include(x => x.Subject)
                         .ToListAsync(cancellationToken);
                //CourseHelper.GetCourseName(_),
                var quzzes = await Db.Quizzes.AsNoTracking()
                    .Where(QuizSpec.ForCourse(request.CourseId))
                    .ToListAsync(cancellationToken);

                var courseStructure = await _contentTreeService.GetCourseStructure(request.CourseId, cancellationToken);

                var courseTemplate = await Db.CourseTemplates.AsNoTracking()
                    .Include(_ => _.Template)
                    .Where(_ => _.CourseId == request.CourseId &&
                                _.Template.Type == Constant.TemplateType.QuizResultsQuizHeader)
                    .SingleAsync(cancellationToken);
                var result = quzzes.Select(_ =>
                {
                    var fullPath = _contentTreePathResolver.ResolveFullPath(_.ContentTreeId,
                        courseStructure);
                    return new Result
                    {
                        QuizId = _.QuizId,
                        Name = courseTemplate == null
                            ? ""
                            : string.Join(" > ", fullPath.Select(_ => _.Value))
                    };
                }).OrderBy(_ => _.Name).ToList();

                return result;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Projections;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Quizzes.Analytic
{
    [PublicAPI]
    public sealed class GetStudentQuizAssignmentResultsQuery : IQuery<GetStudentQuizAssignmentResultsQuery.Result>
    {
        public Guid StudentId { get; init; }
        public Guid SchoolClassId { get; set; }
        public Guid CourseId { get; init; }


        [PublicAPI]
        public record Result
        {
            public QuizAnalyticOutput.Student StudentInfo { get; init; }
            public List<QuizAssignmentResult> Results { get; init; }
            public QuizAnalyticOutput.SchoolClassInfo ClassData { get; init; }
            public QuizAnalyticOutput.SubjectInfo SubjectData { get; init; }

            [PublicAPI]
            public record QuizAssignmentResult
            {
                public Guid QuizStudentAssignmentId { get; init; }
                public Guid QuizAssignmentId { get; init; }
                public Guid QuizId { get; init; }
                public Guid ContentTreeNodeId { get; init; }
                public double Score { get; init; }
                public double Progress { get; init; }
                public DateTime CompletedAt { get; init; }
                public bool NotApplicable { get; init; } // TODO: it's not clear how to handle it
                public string QuizNameHtml { get; init; }
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetStudentQuizAssignmentResultsQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetStudentQuizAssignmentResultsQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));

                return user.IsSchoolTeacher(schoolClass.SchoolId);
            }
        }

        internal sealed class Handler : QueryHandlerBase<GetStudentQuizAssignmentResultsQuery, Result>
        {
	        private readonly IContentTreePathResolver _pathResolver;
	        private readonly ITemplateService _templateService;

            public Handler(IDb db, IContentTreePathResolver pathResolver, ITemplateService templateService) : base(db)
	        {
		        _pathResolver = pathResolver;
		        _templateService = templateService;
            }

            public override async Task<Result> Handle(GetStudentQuizAssignmentResultsQuery request, CancellationToken cancellationToken) => new()
                                                                                                                                            {
                                                                                                                                                StudentInfo = await GetStudentInfo(request, cancellationToken),
                                                                                                                                                Results = await GetQuizResults(request, cancellationToken),
                                                                                                                                                ClassData = await GetClassInfo(request, cancellationToken),
                                                                                                                                                SubjectData = await GetSubjectInfo(request, cancellationToken)
                                                                                                                                            };

            private Task<QuizAnalyticOutput.SubjectInfo> GetSubjectInfo(GetStudentQuizAssignmentResultsQuery request, CancellationToken cancellationToken)
            {
                return Db.Courses
                         .Where(CourseSpec.ById(request.CourseId))
                         .Include(_ => _.Subject)
                         .AsNoTracking()
                         .Select(CourseProjection.ToQuizAnalyticCourse())
                         .SingleAsync(cancellationToken);
            }

            private Task<QuizAnalyticOutput.Student> GetStudentInfo(GetStudentQuizAssignmentResultsQuery request, CancellationToken cancellationToken) => Db.Students
               .Where(StudentSpec.ById(request.StudentId))
               .Select(StudentProjection.ToQuizAnalyticStudent())
               .SingleOrDefaultAsync(cancellationToken);

            private Task<QuizAnalyticOutput.SchoolClassInfo> GetClassInfo(GetStudentQuizAssignmentResultsQuery request, CancellationToken cancellationToken) => Db.SchoolClasses
               .Where(SchoolClassSpec.ById(request.SchoolClassId))
               .Select(SchoolClassProjection.ToQuizAnalyticClass())
               .SingleOrDefaultAsync(cancellationToken);

            private async Task<List<Result.QuizAssignmentResult>> GetQuizResults(GetStudentQuizAssignmentResultsQuery request, CancellationToken cancellationToken)
            {

	            var content = await Db.ContentTree.AsNoTracking()
		            .Include(x => x.ContentTreeMeta)
		            .Where(x => x.CourseId == request.CourseId)
		            .ToListAsync(cancellationToken);
	            var courseStructure = content.ToImmutableDictionary(_ => _.NodeId);
	            var templateEntity = await Db.CourseTemplates.Include(x => x.Template)
		            .Where(x => x.Template.Type == Constant.TemplateType.ClassResultQuizHeader && x.CourseId == request.CourseId)
		            .FirstOrDefaultAsync(cancellationToken);
	            var template = templateEntity?.Template?.Value ?? string.Empty;

	            var assignments = (await Db.QuizStudentAssignments
		            .Include(_ => _.Result)
		            .Include(_ => _.QuizAssignment)
		            .ThenInclude(_ => _.Quiz)
		            .Where(QuizStudentAssignmentSpec.IsCompleted())
		            .Where(QuizStudentAssignmentSpec.ForUser(request.StudentId))
		            .Where(QuizStudentAssignmentSpec.IsPartOfClassAssignment(request.SchoolClassId))
		            .Where(QuizStudentAssignmentSpec.ForCourse(request.CourseId))
		            .OrderBy(_ => _.QuizAssignment.Quiz.QuizId)
		            .ThenBy(_ => _.QuizAssignmentId)
		            .ThenBy(_ => _.Result.CompletedAt)
		            .ToListAsync(cancellationToken))
                    .Select(_ => new Result.QuizAssignmentResult
		            {
			            QuizStudentAssignmentId = _.QuizStudentAssignmentId,
			            QuizAssignmentId = _.QuizAssignmentId,
			            QuizId = _.QuizAssignment.QuizId,
			            ContentTreeNodeId = _.QuizAssignment.Quiz.ContentTreeId,
			            Score = _.Result.Score,
			            Progress = _.Result.Score, // TODO: what Progress means here
			            NotApplicable = false, // TODO: what NotApplicable means here
			            CompletedAt = _.Result.CompletedAt.Value,
			            QuizNameHtml = _templateService.ApplyTemplate(template,
				            _pathResolver.ResolveFullPath(_.QuizAssignment.Quiz.ContentTreeId, courseStructure)
					            .Select(x => new TemplateValue {Index = x.Index, Name = x.Value}).ToList())
		            });

                return (from qr in assignments
                        group qr by qr.QuizAssignmentId
                        into gr
                        select gr.OrderByDescending(_ => _.CompletedAt).First()).ToList();
            }
        }
    }
}

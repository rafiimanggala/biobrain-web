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
    public sealed class GetClassResultsQuery : IQuery<GetClassResultsQuery.Result>
    {
        public Guid CourseId { get; init; }
        public Guid SchoolClassId { get; init; }
        public List<Guid> SelectedFilterNodes { get; init; }
        public bool Anonymise { get; init; }

        [PublicAPI]
        public record Result
        {
            public QuizAnalyticOutput.SchoolClassInfo ClassData { get; init; }
            public QuizAnalyticOutput.SubjectInfo SubjectData { get; init; }
            public List<QuizAnalyticOutput.QuizAssignment> QuizAssignments { get; init; }
            public List<QuizAnalyticOutput.QuizStudentAssignment> QuizStudentAssignments { get; init; }
            public List<QuizAnalyticOutput.Student> Students { get; init; }
            public List<QuizAnalyticOutput.AverageScoreData> AverageScoreInfo { get; init; }
            public List<QuizAnalyticOutput.ProgressData> ProgressInfo { get; init; }
            public List<QuizResultData> QuizResults { get; init; }

            [PublicAPI]
            public record QuizResultData
            {
                public Guid StudentId { get; init; }
                public Guid QuizId { get; init; }
                public Guid QuizAssignmentId { get; init; }
                public bool NotApplicable { get; init; }
                public double Score { get; init; }
                public DateTime CompletedAt { get; init; }
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetClassResultsQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetClassResultsQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                return user.IsSchoolTeacher(schoolClass.SchoolId);
            }
        }

        internal sealed class Handler : QueryHandlerBase<GetClassResultsQuery, Result>
        {
            private readonly IContentTreePathResolver _contentTreePathResolver;
            private readonly ITemplateService _templateService;

            public Handler(IDb db, IContentTreePathResolver contentTreePathResolver, ITemplateService templateService) : base(db)
            {
                _contentTreePathResolver = contentTreePathResolver;
                _templateService = templateService;
            }

            public override async Task<Result> Handle(GetClassResultsQuery request, CancellationToken cancellationToken)
            {
                var quizAssignments = await GetQuizAssignments(request, cancellationToken);
                var quizStudentAssignments = await GetQuizStudentAssignments(quizAssignments, cancellationToken);
                var quizResults = await GetQuizResults(quizAssignments.Select(_ => _.QuizAssignmentId), cancellationToken);
                var students = await GetStudents(request, cancellationToken);

                if (request.Anonymise)
                {
                    var rng = new Random();
                    students = students.OrderBy(_ => rng.Next()).ToList();

                    var idToAnonymousName = new Dictionary<Guid, (string First, string Last)>();
                    for (var i = 0; i < students.Count; i++)
                    {
                        idToAnonymousName[students[i].StudentId] = ($"Student", $"{i + 1}");
                    }

                    students = students.Select((s, i) => s with
                    {
                        FirstName = $"Student",
                        LastName = $"{i + 1}"
                    }).ToList();
                }

                return new Result
                       {
                           ClassData= await GetClass(request, cancellationToken),
                           SubjectData = await GetSubject(request, cancellationToken),
                           Students = students,
                           QuizAssignments = quizAssignments,
                           QuizStudentAssignments = quizStudentAssignments,
                           QuizResults = quizResults,
                           AverageScoreInfo = EvaluateAverageScore(quizResults),
                           ProgressInfo = EvaluateProgressInfo(quizResults)
                       };
            }

            private static List<QuizAnalyticOutput.ProgressData> EvaluateProgressInfo(IEnumerable<Result.QuizResultData> quizResults)
            {
                return (from qr in quizResults
                        group qr by qr.StudentId
                        into gr
                        let progress = gr.Select(_ => _.Score).Average()
                        select new QuizAnalyticOutput.ProgressData
                               {
                                   StudentId = gr.Key,
                                   Progress = progress
                               }).ToList();
            }

            private static List<QuizAnalyticOutput.AverageScoreData> EvaluateAverageScore(IEnumerable<Result.QuizResultData> quizResults)
            {
                return (from qr in quizResults
                        group qr by qr.StudentId
                        into gr
                        let avgScore = gr.Select(_ => _.Score).Average()
                        select new QuizAnalyticOutput.AverageScoreData
                               {
                                   StudentId = gr.Key,
                                   AverageScore = avgScore
                               }).ToList();
            }

            private async Task<List<Result.QuizResultData>> GetQuizResults(IEnumerable<Guid> quizAssignmentIds,
                                                                           CancellationToken cancellationToken)
            {
                var allQuizResults = await Db.QuizStudentAssignments
                                             .Include(_ => _.Result)
                                             .Include(_ => _.QuizAssignment)
                                             .Where(QuizStudentAssignmentSpec.ForQuizAssignments(quizAssignmentIds))
                                             .Where(QuizStudentAssignmentSpec.IsCompleted())
                                             .AsNoTracking()
                                             .Select(_ => new Result.QuizResultData
                                                          {
                                                              QuizAssignmentId = _.QuizAssignmentId,
                                                              QuizId = _.QuizAssignment.QuizId,
                                                              Score = _.Result.Score,
                                                              StudentId = _.AssignedToUserId,
                                                              CompletedAt = _.Result.CompletedAt.Value

                                                              // NotApplicable = TODO: how NotApplicable should be tracked?
                                                          })
                                             .ToListAsync(cancellationToken);

                return (from qr in allQuizResults
                        group qr by new { qr.QuizAssignmentId, qr.StudentId }
                        into gr
                        select gr.OrderByDescending(_ => _.CompletedAt).First()).ToList();
            }

            private async Task<List<QuizAnalyticOutput.QuizAssignment>> GetQuizAssignments(GetClassResultsQuery request, CancellationToken cancellationToken)
            {
                var quizAssignments = await Db.QuizAssignments
                                              .AsNoTracking()
                                              .Include(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                              .Where(QuizAssignmentSpec.ForClass(request.SchoolClassId))
                                              .Where(QuizAssignmentSpec.ForCourse(request.CourseId))
                                              .OrderByDescending(_ => _.AssignedAtUtc)
                                              .Select(QuizAssignmentProjection.ToQuizAnalyticQuizAssignment())
                                              .ToListAsync(cancellationToken);

                var content = await Db.ContentTree.AsNoTracking()
                                      .Include(x => x.ContentTreeMeta)
                                      .Where(x => x.CourseId == request.CourseId)
                                      .ToListAsync(cancellationToken);

                var courseStructure = content.ToImmutableDictionary(_ => _.NodeId);
                var template = await Db.CourseTemplates.Include(x => x.Template)
	                .Where(x => x.Template.Type == Constant.TemplateType.ClassResultQuizHeader && x.CourseId == request.CourseId)
	                .FirstOrDefaultAsync(cancellationToken);

                var result = new List<QuizAnalyticOutput.QuizAssignment>();

                foreach (var quizAssignment in quizAssignments)
                {
                    var path = _contentTreePathResolver.ResolveFullPath(quizAssignment.ContentTreeNodeId, courseStructure);
                    if(request.SelectedFilterNodes != null && request.SelectedFilterNodes.Any() && !request.SelectedFilterNodes.All(_ => path.Any(p => p.NodeId == _))) continue;
                    quizAssignment.Path = path.Select(x => x.Value).ToList();
                    quizAssignment.QuizName =
		                _templateService.ApplyTemplate(template?.Template?.Value ?? string.Empty, path.Select(x => new TemplateValue{Index = x.Index, Name = x.Value}).ToList());
                    result.Add(quizAssignment);
                }

                return result;
            }

            private async Task<List<QuizAnalyticOutput.QuizStudentAssignment>> GetQuizStudentAssignments(List<QuizAnalyticOutput.QuizAssignment> assignments, CancellationToken cancellationToken)
            {
	            var quizStudentAssignments = await Db.QuizStudentAssignments
		            .AsNoTracking()
		            .Where(QuizStudentAssignmentSpec.ForQuizAssignments(assignments.Select(x => x.QuizAssignmentId)))
		            .Select(QuizStudentAssignmentProjection.ToQuizAnalyticQuizStudentAssignment())
		            .ToListAsync(cancellationToken);

	            return quizStudentAssignments;
            }

            private Task<List<QuizAnalyticOutput.Student>> GetStudents(GetClassResultsQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
                         .AsNoTracking()
                         .OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName)
                         .Where(StudentSpec.ForClass(request.SchoolClassId))
                         .Select(StudentProjection.ToQuizAnalyticStudent())
                         .ToListAsync(cancellationToken);
            }

            private Task<QuizAnalyticOutput.SchoolClassInfo> GetClass(GetClassResultsQuery request, CancellationToken cancellationToken) => Db.SchoolClasses
               .Where(SchoolClassSpec.ById(request.SchoolClassId))
               .AsNoTracking()
               .Select(SchoolClassProjection.ToQuizAnalyticClass())
               .SingleAsync(cancellationToken);

            private Task<QuizAnalyticOutput.SubjectInfo> GetSubject(GetClassResultsQuery request, CancellationToken cancellationToken)
            {
                return Db.Courses
                         .Where(CourseSpec.ById(request.CourseId))
                         .Include(_ => _.Subject)
                         .AsNoTracking()
                         .Select(CourseProjection.ToQuizAnalyticCourse())
                         .SingleAsync(cancellationToken);
            }
        }
    }
}

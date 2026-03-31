using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
using BiobrainWebAPI.Values;
using Csv;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Biobrain.Application.Quizzes.Analytic
{
    [PublicAPI]
    public sealed class GetClassResultsCsvQuery : IQuery<GetClassResultsCsvQuery.Result>
    {
        public Guid CourseId { get; init; }
        public Guid SchoolClassId { get; init; }

        [PublicAPI]
        public record Result
        {
	        public string FileUrl { get; set; }

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


        internal sealed class PermissionCheck : PermissionCheckBase<GetClassResultsCsvQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetClassResultsCsvQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                return user.IsSchoolTeacher(schoolClass.SchoolId);
            }
        }

        internal sealed class Handler : QueryHandlerBase<GetClassResultsCsvQuery, Result>
        {
            private readonly IContentTreePathResolver _contentTreePathResolver;
            private readonly ITemplateService _templateService;
            private readonly IConfiguration _configuration;

            public Handler(IDb db, IContentTreePathResolver contentTreePathResolver, ITemplateService templateService, IConfiguration configuration) : base(db)
            {
                _contentTreePathResolver = contentTreePathResolver;
                _templateService = templateService;
                _configuration = configuration;
            }

            public override async Task<Result> Handle(GetClassResultsCsvQuery request, CancellationToken cancellationToken)
            {
                var quizAssignments = await GetQuizAssignments(request, cancellationToken);
                var quizStudentAssignments = await GetQuizStudentAssignments(quizAssignments, cancellationToken);
                var quizResults = await GetQuizResults(quizAssignments.Select(_ => _.QuizAssignmentId), cancellationToken);
                var classData = await GetClass(request, cancellationToken);
                var students = await GetStudents(request, cancellationToken);
                var averageScoreInfo = EvaluateAverageScore(quizResults);
                var progressInfo = EvaluateProgressInfo(quizResults);

                var fileName = Guid.NewGuid() + ".csv";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink, fileName);

                var header = GetHeader(quizAssignments, classData);
                var rows = GetRows(students, averageScoreInfo, progressInfo, quizAssignments, quizStudentAssignments, quizResults);


                var csv = CsvWriter.WriteToText(header, rows);
                await File.WriteAllTextAsync(filePath, csv, cancellationToken);

                return new Result
	                {FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString()};
            }

            private string[] GetHeader(List<QuizAnalyticOutput.QuizAssignment> quizAssignments, QuizAnalyticOutput.SchoolClassInfo classData)
            {
	            var header = new List<string> { classData.SchoolClassName, "", "Average Quiz Score", "Student Progress" };

                quizAssignments.ForEach(x => header.Add(x.QuizName.Replace("<div>", "").Replace("</div>","\n")));

	            return header.ToArray();
            }

            private List<string[]> GetRows(
	            List<QuizAnalyticOutput.Student> students, 
	            List<QuizAnalyticOutput.AverageScoreData> averageScores, 
	            List<QuizAnalyticOutput.ProgressData> progressInfo, 
	            List<QuizAnalyticOutput.QuizAssignment> quizAssignments,
	            List<QuizAnalyticOutput.QuizStudentAssignment> quizStudentAssignments,
	            List<Result.QuizResultData> quizResults)
            {
	            var rows = new List<string[]>();

                students.ForEach(student =>
                {
	                var row = new List<string> {student.LastName, student.FirstName};
	                // Name
	                // Average Quiz Score
                    var score = averageScores.FirstOrDefault(x => x.StudentId == student.StudentId);
                    row.Add(score == null ? "N/A" : score.AverageScore.ToString("F0"));
                    // Progress
                    var progress = progressInfo.FirstOrDefault(x => x.StudentId == student.StudentId);
                    row.Add(progress == null ? "-" : progress.Progress.ToString("F0"));

                    quizAssignments.ForEach(qa =>
                    {
	                    var studentAssignment =
		                    quizStudentAssignments.FirstOrDefault(_ => _.AssignedToUserId == student.StudentId);
	                    var quizResult = quizResults.FirstOrDefault(_ =>
		                    _.StudentId == student.StudentId && _.QuizAssignmentId == qa.QuizAssignmentId);

	                    row.Add(quizResult == null
		                    ? studentAssignment == null ? "N/A" : "A"
		                    : $"{quizResult.Score:F0} %");
                    });

                    rows.Add(row.ToArray());
                });

	            return rows;
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
                                                          })
                                             .ToListAsync(cancellationToken);

                return (from qr in allQuizResults
                        group qr by new { qr.QuizAssignmentId, qr.StudentId }
                        into gr
                        select gr.OrderByDescending(_ => _.CompletedAt).First()).ToList();
            }

            private async Task<List<QuizAnalyticOutput.QuizAssignment>> GetQuizAssignments(GetClassResultsCsvQuery request, CancellationToken cancellationToken)
            {
                var quizAssignments = await Db.QuizAssignments
                                              .AsNoTracking()
                                              .Include(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                              .Where(QuizAssignmentSpec.ForClass(request.SchoolClassId))
                                              .Where(QuizAssignmentSpec.ForCourse(request.CourseId))
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

                foreach (var quizAssignment in quizAssignments)
                {
                    var path = _contentTreePathResolver.ResolveFullPath(quizAssignment.ContentTreeNodeId, courseStructure);
                    quizAssignment.Path = path.Select(x => x.Value).ToList();
                    quizAssignment.QuizName =
		                _templateService.ApplyTemplate(template?.Template?.Value ?? string.Empty, path.Select(x => new TemplateValue{Index = x.Index, Name = x.Value}).ToList());
                }

                return quizAssignments.OrderByDescending(x => x.DateUtc).ToList();
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

            private Task<List<QuizAnalyticOutput.Student>> GetStudents(GetClassResultsCsvQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
                         .AsNoTracking()
                         .Where(StudentSpec.ForClass(request.SchoolClassId))
                         .OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName)
                         .Select(StudentProjection.ToQuizAnalyticStudent())
                         .ToListAsync(cancellationToken);
            }

            private Task<QuizAnalyticOutput.SchoolClassInfo> GetClass(GetClassResultsCsvQuery request, CancellationToken cancellationToken) => Db.SchoolClasses
               .Where(SchoolClassSpec.ById(request.SchoolClassId))
               .AsNoTracking()
               .Select(SchoolClassProjection.ToQuizAnalyticClass())
               .SingleAsync(cancellationToken);

            //private Task<QuizAnalyticOutput.SubjectInfo> GetSubject(GetClassResultsCsvQuery request, CancellationToken cancellationToken)
            //{
            //    return Db.Courses
            //             .Where(CourseSpec.ById(request.CourseId))
            //             .Include(_ => _.Subject)
            //             .AsNoTracking()
            //             .Select(CourseProjection.ToQuizAnalyticCourse())
            //             .SingleAsync(cancellationToken);
            //}
        }
    }
}

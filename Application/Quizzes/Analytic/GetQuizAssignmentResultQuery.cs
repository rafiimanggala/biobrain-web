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
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Quizzes.Analytic
{
    [PublicAPI]
    public sealed class GetQuizAssignmentResultQuery : IQuery<GetQuizAssignmentResultQuery.Result>
    {
        public Guid QuizAssignmentId { get; init; }

        [PublicAPI]
        public record Result
        {
            public QuizAnalyticOutput.SchoolClassInfo ClassData { get; init; }
            public QuizAnalyticOutput.SubjectInfo SubjectData { get; init; }
            public Guid QuizId { get; init; }
            public Guid ContentTreeNodeId { get; init; }
            public Guid QuizAssignmentId { get; init; }
            public List<QuizAnalyticOutput.Student> Students { get; init; }
            public List<QuizAnalyticOutput.AverageScoreData> AverageScoreInfo { get; init; }
            public List<QuizAnalyticOutput.ProgressData> ProgressInfo { get; init; }
            public List<QuestionResult> QuestionResults { get; init; }
            public List<Question> Questions { get; init; }
            public string QuizName { get; set; }

            [PublicAPI]
            public record QuestionResult
            {
                public Guid StudentId { get; init; }
                public Guid QuestionId { get; init; }
                public bool IsCorrect { get; init; }
            }

            [PublicAPI]
            public record Question
            {
                public Guid QuestionId { get; init; }
                public string Text { get; init; }
                public string Header { get; init; }
                public long QuestionTypeCode { get; init; }
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetQuizAssignmentResultQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetQuizAssignmentResultQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var quizAssignment = _db.QuizAssignments.GetSingle(QuizAssignmentSpec.ById(request.QuizAssignmentId));
                if (quizAssignment.SchoolClassId.HasValue)
                {
                    var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(quizAssignment.SchoolClassId.Value));
                    return user.IsSchoolTeacher(schoolClass.SchoolId);
                }

                // TODO: for individual teachers add check that teacher has access only to quizzes created by that teacher (or something like this).
                // Temp workaround: return true (in any case we don't have such quiz assignments right now).
                // var teacher = _db.Teachers.GetSingle(TeacherSpec.ByUserAccount(user.UserId));
                // return quizAssignment.AssignedByTeacherId == teacher.TeacherId;
                return true;
            }
        }


        private class Handler : QueryHandlerBase<GetQuizAssignmentResultQuery, Result>
        {
            private readonly ITemplateService _templateService;
            private readonly IContentTreePathResolver _contentTreePathResolver;

            public Handler(IDb db, ITemplateService templateService, IContentTreePathResolver contentTreePathResolver) : base(db)
            {
                _templateService = templateService;
                _contentTreePathResolver = contentTreePathResolver;
            }

            public override async Task<Result> Handle(GetQuizAssignmentResultQuery request, CancellationToken cancellationToken)
            {
                var quizAssignment = await Db.QuizAssignments
                                             .Include(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                             .Where(QuizAssignmentSpec.ById(request.QuizAssignmentId))
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(cancellationToken);

                var template = await Db.CourseTemplates.Include(x => x.Template)
                    .Where(x => x.Template.Type == Constant.TemplateType.ClassResultQuizHeader && x.CourseId == quizAssignment.Quiz.ContentTreeNode.CourseId)
                    .FirstOrDefaultAsync(cancellationToken);
                var content = await Db.ContentTree.AsNoTracking()
                    .Include(x => x.ContentTreeMeta)
                    .Where(x => x.CourseId == quizAssignment.Quiz.ContentTreeNode.CourseId)
                    .ToListAsync(cancellationToken);

                var courseStructure = content.ToImmutableDictionary(_ => _.NodeId);
                var path = _contentTreePathResolver.ResolveFullPath(quizAssignment.Quiz.ContentTreeId, courseStructure);

                var results = await GetAnalytic(quizAssignment, cancellationToken);

                return new Result
                       {
                           ClassData = await GetSchoolClass(quizAssignment, cancellationToken),
                           SubjectData = await GetSubjectData(quizAssignment.QuizId),
                           Questions = await GetQuestions(results.SelectMany(_ => _.QuestionResults), quizAssignment.Quiz.QuizId, cancellationToken),
                           QuizId = quizAssignment.Quiz.QuizId,
                           ContentTreeNodeId = quizAssignment.Quiz.ContentTreeId,
                           QuizAssignmentId = quizAssignment.QuizAssignmentId,
                           Students = await GetStudents(quizAssignment, cancellationToken),
                           AverageScoreInfo = ToAvgScoreData(results),
                           ProgressInfo = ToProgressData(results),
                           QuestionResults = ToQuestionResults(results),
                           QuizName = _templateService.ApplyTemplate(template?.Template?.Value ?? string.Empty, path.Select(x => new TemplateValue { Index = x.Index, Name = x.Value }).ToList())
            };
            }

            private async Task<List<Result.Question>> GetQuestions(IEnumerable<QuizResultQuestionEntity> questionResults,
                                                             Guid quizId,
                                                             CancellationToken cancellationToken)
            {
                List<Guid> questionIds = [..questionResults.Select(x => x.QuestionId).Distinct()];

                var questions = await Db.Questions
                                         .Where(QuestionSpec.ByIds(questionIds))
                                         .Include(x => x.QuizQuestions)
                                         .Select(x => new
                                                      {
                                                          x.QuestionId,
                                                          x.Header,
                                                          x.Text,
                                                          x.QuestionTypeCode,
                                                          Order = x.QuizQuestions
                                                                   .Where(z => z.QuizId == quizId && z.QuestionId == x.QuestionId)
                                                                   .Select(z => z.Order)
                                                                   .FirstOrDefault()
                                                      })
                                         .OrderBy(x => x.Order)
                                         .ToListAsync(cancellationToken);

                return
                [
                    ..questions.OrderBy(x => int.TryParse(x.Header.Substring(1), out int headerOrder) ? headerOrder : x.Order)
                               .Select(x => new Result.Question
                                            {
                                                QuestionId = x.QuestionId,
                                                Header = x.Header,
                                                Text = x.Text,
                                                QuestionTypeCode = x.QuestionTypeCode
                                            })
                ];
            }

            private static List<Result.QuestionResult> ToQuestionResults(
                IEnumerable<(Guid StudentId, double Score, ICollection<QuizResultQuestionEntity> QuestionResults)> results)
                => (from res in results
                    let studentId = res.StudentId
                    from question in res.QuestionResults
                    select new Result.QuestionResult
                           {
                               StudentId = studentId,
                               QuestionId = question.QuestionId,
                               IsCorrect = question.IsCorrect
                           }).ToList();

            private static List<QuizAnalyticOutput.ProgressData> ToProgressData(
                IEnumerable<(Guid StudentId, double Score, ICollection<QuizResultQuestionEntity> QuestionResults)> results)
            {
                return results.Select(_ => new QuizAnalyticOutput.ProgressData
                                           {
                                               Progress = _.Score,
                                               StudentId = _.StudentId
                                           })
                              .ToList();
            }

            private static List<QuizAnalyticOutput.AverageScoreData> ToAvgScoreData(
                IEnumerable<(Guid StudentId, double Score, ICollection<QuizResultQuestionEntity> QuestionResults)> results)
            {
                return results.Select(_ => new QuizAnalyticOutput.AverageScoreData
                                           {
                                               AverageScore = _.Score,
                                               StudentId = _.StudentId,
                                               NotApplicable = string.Empty
                                           })
                              .ToList();
            }

            private async Task<List<(Guid StudentId, double Score, ICollection<QuizResultQuestionEntity> QuestionResults)>>
                GetAnalytic(QuizAssignmentEntity quizAssignment, CancellationToken cancellationToken)
            {
                var allResults = await Db.QuizStudentAssignments
                                         .Where(QuizStudentAssignmentSpec.ForQuizAssignment(quizAssignment.QuizAssignmentId))
                                         .Where(QuizStudentAssignmentSpec.IsCompleted())
                                         .Include(_ => _.Result)
                                         .ThenInclude(_ => _.Questions)
                                         .AsNoTracking()
                                         .ToListAsync(cancellationToken);

                return (from qr in allResults
                        group qr by qr.AssignedToUserId
                        into gr
                        let finalResult = gr.OrderByDescending(_ => _.Result.CompletedAt).First()
                        select (
                                   StudentId: gr.Key,
                                   finalResult.Result.Score,
                                   finalResult.Result.Questions

                                   // TODO: How to track Not Applicable
                               )).ToList();
            }

            private Task<QuizAnalyticOutput.SubjectInfo> GetSubjectData(Guid quizId)
            {
                return Db.Quizzes
                         .Where(QuizSpec.ByIds(new[] {quizId}))
                         .Include(_ => _.ContentTreeNode).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                         .Select(_ => new QuizAnalyticOutput.SubjectInfo
                                      {
                                          CourseId = _.ContentTreeNode.CourseId,
                                          SubjectName = _.ContentTreeNode.Course.Subject.Name,
                                      })
                         .SingleOrDefaultAsync();
            }

            private Task<QuizAnalyticOutput.SchoolClassInfo> GetSchoolClass([NotNull] QuizAssignmentEntity quizAssignment, CancellationToken cancellationToken)
            {
                if (quizAssignment is null)
                    throw new ArgumentNullException(nameof(quizAssignment));

                if (!quizAssignment.SchoolClassId.HasValue)
                {
                    return Task.FromResult<QuizAnalyticOutput.SchoolClassInfo>(null);
                }

                return Db.SchoolClasses
                         .Where(SchoolClassSpec.ById(quizAssignment.SchoolClassId.Value))
                         .AsNoTracking()
                         .Select(SchoolClassProjection.ToQuizAnalyticClass())
                         .SingleOrDefaultAsync(cancellationToken);
            }

            private Task<List<QuizAnalyticOutput.Student>> GetStudents([NotNull] QuizAssignmentEntity quizAssignment, CancellationToken cancellationToken)
            {
                if (quizAssignment is null)
                    throw new ArgumentNullException(nameof(quizAssignment));

                return Db.Students
                         .Include(_ => _.User)
                         .Where(StudentSpec.ForQuizAssignment(quizAssignment.QuizAssignmentId))
                         .AsNoTracking()
                         .Select(StudentProjection.ToQuizAnalyticStudent())
                         .ToListAsync(cancellationToken);
            }
        }
    }
}

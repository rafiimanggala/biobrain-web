using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.ContentTreeService;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Content;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetQuizResultHistory
{
    [PublicAPI]
    public class GetQuizResultHistoryQuery : ICommand<GetQuizResultHistoryQuery.Result>
    {
	    public Guid CourseId { get; set; }

	    [PublicAPI]
        public class Result
        {
	        public double AverageQuizRate { get; set; }
	        public double QuizzesCompletedRate { get; set; }
	        public List<QuizResult> QuizResults { get; set; } = new();
        }

        public record QuizResult(Guid QuizResultId, Guid QuizId, Guid CourseId, Guid UnitId, Guid NodeId, Guid? ParentNodeId, List<string> Path, List<string> NameLines, double Score, DateTime Date);


        internal class Validator : ValidatorBase<GetQuizResultHistoryQuery>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizResultHistoryQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GetQuizResultHistoryQuery request, IUserSecurityInfo user) => user.IsStudent();
        }


        internal class Handler : CommandHandlerBase<GetQuizResultHistoryQuery, Result>
        {
	        private readonly ISessionContext _sessionContext;
            private readonly IContentTreePathResolver _contentTreePathResolver;
            private readonly IContentTreeService _contentTreeService;
            private readonly ITemplateService _templateService;

            public Handler(IDb db, ISessionContext sessionContext, IContentTreePathResolver contentTreePathResolver, IContentTreeService contentTreeService, ITemplateService templateService) : base(db)
            {
                this._sessionContext = sessionContext;
                _contentTreePathResolver = contentTreePathResolver;
                _contentTreeService = contentTreeService;
                _templateService = templateService;
            }

            public override async Task<Result> Handle(GetQuizResultHistoryQuery request,
	            CancellationToken cancellationToken)
            {
	            var userId = _sessionContext.GetUserId();
	            var quizAssignments = await Db.QuizStudentAssignments
		            .Include(_ => _.QuizAssignment)
		            .ThenInclude(x => x.Quiz)
		            .ThenInclude(x => x.ContentTreeNode)
		            .Include(_ => _.Result)
		            .ThenInclude(x => x.Questions)
		            .Where(_ => _.AssignedToUserId == userId &&
		                        _.QuizAssignment.Quiz.ContentTreeNode.CourseId == request.CourseId &&
		                        _.Result != null && _.Result.CompletedAt != null)
		            .OrderByDescending(x => x.Result.CompletedAt)
		            .ToListAsync(cancellationToken);
	            var quizzesCount = await Db.Quizzes.AsNoTracking()
		            .Include(x => x.ContentTreeNode)
		            .Where(x => x.ContentTreeNode.CourseId == request.CourseId)
		            .CountAsync(cancellationToken: cancellationToken);

                var courseStructure = await _contentTreeService.GetCourseStructure(request.CourseId, cancellationToken);

                var courseTemplate = await Db.CourseTemplates.AsNoTracking()
                    .Include(_ => _.Template)
                    .Where(_ => _.CourseId == request.CourseId &&
                                _.Template.Type == Constant.TemplateType.QuizResultsQuizHeader)
                    .SingleAsync(cancellationToken);

                return new Result
                {
                    AverageQuizRate = Math.Round(quizAssignments.Sum(x => x.Result.Questions.Any()
                                                     ? (((double)x.Result.Questions.Count(y => y.IsCorrect)) /
                                                        (double)x.Result.Questions.Count)
                                                     : 0) /
                                                 (quizAssignments.Any() ? quizAssignments.Count : 1), 2),
                    QuizzesCompletedRate = Math.Round(
                        (double)quizAssignments.Select(x => x.QuizAssignment.QuizId).Distinct().Count() /
                        (quizzesCount == 0 ? 1 : quizzesCount), 2),
                    QuizResults = quizAssignments.Where(x => x.Result.CompletedAt != null).Select(x =>
                        {
                            var fullPath = _contentTreePathResolver.ResolveFullPath(x.QuizAssignment.Quiz.ContentTreeId,
                                courseStructure);
                            var nameLines = courseTemplate == null
                                ? fullPath.OrderBy(x => x.Index).Select(x => x.Value).ToList()
                                : _templateService.ApplyTemplate(courseTemplate.Template.Value,
                                    fullPath.Select(x => new TemplateValue { Index = x.Index, Name = x.Value })
                                        .ToList()).Split('\n').ToList();
                            return new QuizResult(
                                x.QuizStudentAssignmentId,
                                x.QuizAssignment.QuizId,
                                x.QuizAssignment.Quiz.ContentTreeNode.CourseId,
                                GetUnitId(x.QuizAssignment.Quiz.ContentTreeId, courseStructure),
                                x.QuizAssignment.Quiz.ContentTreeId, x.QuizAssignment.Quiz.ContentTreeNode.ParentId,
                                _contentTreePathResolver.ResolvePath(x.QuizAssignment.Quiz.ContentTreeId,
                                    courseStructure),
                                nameLines,
                                x.Result.Questions.Any() ? x.Result.Questions.Count(y => y.IsCorrect) : 0,
                                x.Result.CompletedAt ?? DateTime.MinValue);
                        }
                    ).ToList()
                };
            }
        }

        private static Guid GetUnitId(Guid nodeId, IDictionary<Guid, ContentTreeEntity> courseStructure)
        {
            var node = courseStructure[nodeId];
            while (node.ParentId != null)
                node = courseStructure[node.ParentId.Value];

            return node.NodeId;
        }
    }
}

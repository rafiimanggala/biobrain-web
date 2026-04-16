using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Templates;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.AssignedWork.GetTeacherAssignedWork
{
    [PublicAPI]
    public sealed class GetTeacherAssignedWorkQuery : IQuery<GetTeacherAssignedWorkQuery.Result>
    {
        //public Guid RootContentNodeId { get; init; }
        public Guid UserId { get; init; }
        public Guid SchoolClassId { get; init; }


        [PublicAPI]
        public record Result
        {
            public ImmutableList<ActiveQuizAssignment> Quizzes { get; init; }
            public ImmutableList<ActiveLearningMaterialAssignment> LearningMaterials { get; init; }
        }


        [PublicAPI]
        public record ActiveQuizAssignment
        {
            public Guid QuizAssignmentId { get; init; }

            public Guid NodeId { get; init; }

            public ImmutableList<string> Path { get; init; }

            public string Title { get; set; }

            public DateTime? DueAt { get; init; }

            public DateTime? AssignedAt { get; init; }

            public int StudentAssigned { get; init; }

            public Constant.QuizAssignmentStatus Status { get; init; }
        }


        [PublicAPI]
        public record ActiveLearningMaterialAssignment
        {
            public Guid LearningMaterialAssignmentId { get; init; }

            public Guid NodeId { get; init; }

            public ImmutableList<string> Path { get; init; }

            public string Title { get; set; }

            public DateTime? DueAt { get; init; }

            public DateTime? AssignedAt { get; init; }

            public int StudentAssigned { get; init; }

            public Constant.QuizAssignmentStatus Status { get; init; }
        }

        internal sealed class Validator : ValidatorBase<GetTeacherAssignedWorkQuery>
        {
            public Validator(IDb db) : base(db)
            {
                //RuleFor(_ => _.RootContentNodeId).ExistsInTable(Db.ContentTree);
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Teachers);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetTeacherAssignedWorkQuery>
        {
            private readonly ISessionContext _sessionContext;
            private readonly IDb _db;

            public PermissionCheck(IDb db, ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
            {
                _sessionContext = sessionContext;
                _db = db;
            }

            protected override bool CanExecute(GetTeacherAssignedWorkQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.Single(_ => _.SchoolClassId == request.SchoolClassId);
                return _sessionContext.GetUserId() == request.UserId && _sessionContext.IsFromSchool(schoolClass.SchoolId);
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetTeacherAssignedWorkQuery, Result>
        {
            private readonly IContentTreePathResolver contentTreePathResolver;
            private readonly ITemplateService _templateService;

            public Handler(IDb db, IContentTreePathResolver contentTreePathResolver, ITemplateService templateService) : base(db)
            {
                this.contentTreePathResolver = contentTreePathResolver;
                this._templateService = templateService;
            }

            public override async Task<Result> Handle(GetTeacherAssignedWorkQuery request, CancellationToken cancellationToken)
            {
                var schoolCLass = await Db.SchoolClasses.Where(SchoolClassSpec.ById(request.SchoolClassId))
                    .SingleAsync(cancellationToken);
                var courseStructure = await GetCourseStructure(schoolCLass.CourseId, cancellationToken);

                var template = await Db.CourseTemplates.Include(x => x.Template)
                    .Where(x => x.Template.Type == Constant.TemplateType.BookmarkPathHeader && x.CourseId == schoolCLass.CourseId)
                    .FirstOrDefaultAsync(cancellationToken);

                return new Result
                       {
                           LearningMaterials = await GetActiveAssignedMaterials(request, template, courseStructure, cancellationToken),
                           Quizzes = await GetActiveQuizAssignments(request, template, courseStructure, cancellationToken)
                       };
            }

            private async Task<ImmutableDictionary<Guid, ContentTreeEntity>> GetCourseStructure(Guid courseId, CancellationToken cancellationToken)
            {
                var content = await Db.ContentTree
                                      .Include(_ => _.ContentTreeMeta)
                                      .AsNoTracking()
                                      .Where(ContentTreeSpec.ForCourse(courseId))
                                      .ToListAsync(cancellationToken);

                return content.ToImmutableDictionary(_ => _.NodeId);
            }

            private async Task<ImmutableList<ActiveQuizAssignment>> GetActiveQuizAssignments(GetTeacherAssignedWorkQuery request,
                                                                                             CourseTemplateEntity titleTemplate,
                                                                                             ImmutableDictionary<Guid, ContentTreeEntity> courseStructure,
                                                                                             CancellationToken cancellationToken)
            {
                var quizAssignments = await Db.QuizAssignments
                                              .AsNoTracking()
                                              .Include(_ => _.QuizStudentAssignments)
                                              .ThenInclude(_ => _.Result)
                                              .Include(_ => _.Quiz)
                                              .ThenInclude(_ => _.ContentTreeNode)
                                              .Where(QuizAssignmentSpec.ForClass(request.SchoolClassId))
                                              .Where(QuizAssignmentSpec.AssignedByTeacher(request.UserId))
                                              .Where(QuizAssignmentSpec.HasClassAssignment())
                                              .ToListAsync(cancellationToken);

                return
                    quizAssignments /*.Where(_ => _contentTreePathResolver.EvalRoot(_.QuizAssignment.Quiz.ContentTreeId, courseStructure) == request.RootContentNodeId)*/
                        .Where(_ => _.QuizStudentAssignments.Any(_ => _.Result?.CompletedAt == null))
                        .Select(_ =>
                        {
                            var fullPath = contentTreePathResolver.ResolveFullPath(_.Quiz.ContentTreeId, courseStructure);
                            var resolvedPath = contentTreePathResolver.ResolvePath(_.Quiz.ContentTreeId, courseStructure);

                            // For teacher custom quizzes, replace the last path entry with the quiz name
                            if (_.Quiz.Type == QuizType.TeacherCustom
                                && !string.IsNullOrEmpty(_.Quiz.Name)
                                && resolvedPath.Count > 0)
                            {
                                resolvedPath[resolvedPath.Count - 1] = _.Quiz.Name;
                            }

                            return new ActiveQuizAssignment
                            {
                                QuizAssignmentId = _.QuizAssignmentId,
                                NodeId = _.Quiz.ContentTreeId,
                                Path = resolvedPath.ToImmutableList(),
                                Title = _templateService.ApplyTemplate(titleTemplate?.Template?.Value ?? string.Empty,
                                    fullPath.Select(x => new TemplateValue { Index = x.Index, Name = x.Value }).ToList()),
                                DueAt = _.DueAtUtc,
                                AssignedAt = _.AssignedAtUtc,
                                StudentAssigned = _.QuizStudentAssignments.Count,
                                Status = _.QuizStudentAssignments.All(_ => _.Result?.CompletedAt != null)
                                    ? Constant.QuizAssignmentStatus.Complete
                                    : _.QuizStudentAssignments.Any(_ => _.Result?.CompletedAt != null)
                                        ? Constant.QuizAssignmentStatus.PartiallyComplete
                                        : Constant.QuizAssignmentStatus.Assigned
                            };
                        })
                        .ToImmutableList();
            }

            private async Task<ImmutableList<ActiveLearningMaterialAssignment>> GetActiveAssignedMaterials(GetTeacherAssignedWorkQuery request,
                                                                                                           CourseTemplateEntity titleTemplate,
                                                                                                           ImmutableDictionary<Guid, ContentTreeEntity> courseStructure,
                                                                                                           CancellationToken cancellationToken)
            {
                var assignments = await Db.LearningMaterialAssignments
                    .Include(_ => _.UserAssignments)
                    .Where(LearningMaterialAssignmentSpec.AssignedByTeacher(request.UserId))
                    .Where(LearningMaterialAssignmentSpec.ByClassId(request.SchoolClassId))
                    .ToListAsync(cancellationToken);

                return assignments/*.Where(_ => _contentTreePathResolver.EvalRoot(_.LearningMaterialAssignment.ContentTreeNodeId, courseStructure) == request.RootContentNodeId)*/
                    // Not completed
                    .Where(_ => _.UserAssignments.Any(_ => _.CompletedAtUtc == null))
                                  .Select(_ =>
                                  {
                                      var path = contentTreePathResolver.ResolveFullPath(_.ContentTreeNodeId, courseStructure);
                                      return new ActiveLearningMaterialAssignment
                                      {
                                          LearningMaterialAssignmentId = _.LearningMaterialAssignmentId,
                                          NodeId = _.ContentTreeNodeId,
                                          Path = contentTreePathResolver.ResolvePath(_.ContentTreeNodeId, courseStructure).ToImmutableList(),
                                          Title = _templateService.ApplyTemplate(titleTemplate?.Template?.Value ?? string.Empty,
                                              path.Select(x => new TemplateValue { Index = x.Index, Name = x.Value }).ToList()),
                                          AssignedAt = _.AssignedAtUtc,
                                          DueAt = _.DueAtUtc,
                                          StudentAssigned = _.UserAssignments.Count,
                                          Status = _.UserAssignments.All(_ => _.CompletedAtUtc != null)
                                              ? Constant.QuizAssignmentStatus.Complete
                                              : _.UserAssignments.Any(_ => _.CompletedAtUtc != null)
                                                  ? Constant.QuizAssignmentStatus.PartiallyComplete
                                                  : Constant.QuizAssignmentStatus.Assigned
                                      };
                                  })
                                  .ToImmutableList();
            }
        }
    }
}

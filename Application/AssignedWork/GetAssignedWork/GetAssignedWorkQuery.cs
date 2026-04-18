using System;
using System.Collections.Generic;
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


namespace Biobrain.Application.AssignedWork.GetAssignedWork
{
    [PublicAPI]
    public sealed class GetAssignedWorkQuery : IQuery<GetAssignedWorkQuery.Result>
    {
        //public Guid RootContentNodeId { get; init; }
        public Guid UserId { get; init; }
        public Guid CourseId { get; init; }


        [PublicAPI]
        public record Result
        {
            public ImmutableList<ActiveQuizAssignment> Quizzes { get; init; }
            public ImmutableList<ActiveLearningMaterialAssignment> LearningMaterials { get; init; }
        }


        [PublicAPI]
        public record ActiveQuizAssignment
        {
            public Guid QuizStudentAssignmentId { get; init; }

            public Guid NodeId { get; init; }

            public ImmutableList<string> Path { get; init; }

            public List<string> NameLines { get; init; }

            public DateTime? DueAt { get; init; }

            public DateTime? AssignedAt { get; init; }

            public bool IsCustomQuiz { get; init; }
        }


        [PublicAPI]
        public record ActiveLearningMaterialAssignment
        {
            public Guid LearningMaterialUserAssignmentId { get; init; }

            public Guid NodeId { get; init; }

            public ImmutableList<string> Path { get; init; }

            public List<string> NameLines { get; init; }

            public DateTime? DueAt { get; init; }

            public DateTime? AssignedAt { get; init; }
        }

        internal sealed class Validator : ValidatorBase<GetAssignedWorkQuery>
        {
            public Validator(IDb db) : base(db)
            {
                //RuleFor(_ => _.RootContentNodeId).ExistsInTable(Db.ContentTree);
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Students);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetAssignedWorkQuery>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(GetAssignedWorkQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _sessionContext.GetUserId() == request.UserId;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetAssignedWorkQuery, Result>
        {
            private readonly IContentTreePathResolver _contentTreePathResolver;
            private readonly ITemplateService _templateService;

            public Handler(IDb db, IContentTreePathResolver contentTreePathResolver, ITemplateService templateService) : base(db)
            {
                _contentTreePathResolver = contentTreePathResolver;
                _templateService = templateService;
            }

            public override async Task<Result> Handle(GetAssignedWorkQuery request, CancellationToken cancellationToken)
            {
                var courseStructure = await GetCourseStructure(request, cancellationToken);
                var courseTemplate = await Db.CourseTemplates.AsNoTracking()
                    .Include(_ => _.Template)
                    .Where(_ => _.CourseId == request.CourseId &&
                                _.Template.Type == Constant.TemplateType.QuizResultsQuizHeader)
                    .SingleAsync(cancellationToken);

                return new Result
                       {
                           LearningMaterials = await GetActiveAssignedMaterials(request, courseStructure, courseTemplate, cancellationToken),
                           Quizzes = await GetActiveQuizAssignments(request, courseStructure, courseTemplate, cancellationToken)
                       };
            }

            private async Task<ImmutableDictionary<Guid, ContentTreeEntity>> GetCourseStructure(GetAssignedWorkQuery request, CancellationToken cancellationToken)
            {
                var content = await Db.ContentTree
                                      .Include(_ => _.ContentTreeMeta)
                                      .AsNoTracking()
                                      .Where(ContentTreeSpec.ForCourse(request.CourseId))
                                      .ToListAsync(cancellationToken);

                return content.ToImmutableDictionary(_ => _.NodeId);
            }

            private async Task<ImmutableList<ActiveQuizAssignment>> GetActiveQuizAssignments(GetAssignedWorkQuery request,
                                                                                             ImmutableDictionary<Guid, ContentTreeEntity> courseStructure,
                                                                                             CourseTemplateEntity courseTemplate,
                                                                                             CancellationToken cancellationToken)
            {
                var quizAssignments = await Db.QuizStudentAssignments
                                              .AsNoTracking()
                                              .Include(_ => _.QuizAssignment)
                                              .ThenInclude(_ => _.Quiz)
                                              .ThenInclude(_ => _.ContentTreeNode)
                                              .Where(QuizStudentAssignmentSpec.ForCourse(request.CourseId))
                                              .Where(QuizStudentAssignmentSpec.ForUser(request.UserId))
                                              .Where(QuizStudentAssignmentSpec.HasClassAssignment())
                                              .Where(QuizStudentAssignmentSpec.NotCompleted())
                                              .ToListAsync(cancellationToken);




                return quizAssignments/*.Where(_ => _contentTreePathResolver.EvalRoot(_.QuizAssignment.Quiz.ContentTreeId, courseStructure) == request.RootContentNodeId)*/
                                      .Select(_ =>
                                      {
                                          var fullPath = _contentTreePathResolver.ResolveFullPath(_.QuizAssignment.Quiz.ContentTreeId,
                                              courseStructure);
                                          var nameLines = courseTemplate == null
                                              ? fullPath.OrderBy(x => x.Index).Select(x => x.Value).ToList()
                                              : _templateService.ApplyTemplate(courseTemplate.Template.Value,
                                                  fullPath.Select(x => new TemplateValue { Index = x.Index, Name = x.Value })
                                                      .ToList()).Split('\n').ToList();

                                          var path = _contentTreePathResolver
                                              .ResolvePath(_.QuizAssignment.Quiz.ContentTreeId, courseStructure);

                                          // For teacher custom quizzes, replace the last path entry with the quiz name
                                          if (_.QuizAssignment.Quiz.Type == QuizType.TeacherCustom
                                              && !string.IsNullOrEmpty(_.QuizAssignment.Quiz.Name)
                                              && path.Count > 0)
                                          {
                                              path[path.Count - 1] = _.QuizAssignment.Quiz.Name;
                                          }

                                          return new ActiveQuizAssignment
                                          {
                                              QuizStudentAssignmentId = _.QuizStudentAssignmentId,
                                              NodeId = _.QuizAssignment.Quiz.ContentTreeId,
                                              Path = path.ToImmutableList(),
                                              NameLines = nameLines,
                                              DueAt = _.DueAtUtc,
                                              AssignedAt = _.AssignedAtUtc,
                                              IsCustomQuiz = _.QuizAssignment.Quiz.Type == QuizType.TeacherCustom
                                          };
                                      })
                                      .ToImmutableList();
            }

            private async Task<ImmutableList<ActiveLearningMaterialAssignment>> GetActiveAssignedMaterials(GetAssignedWorkQuery request,
                                                                                                           ImmutableDictionary<Guid, ContentTreeEntity> courseStructure,
                                                                                                           CourseTemplateEntity courseTemplate,
                                                                                                           CancellationToken cancellationToken)
            {
                var assignments = await Db.LearningMaterialUserAssignments
                                          .Include(_ => _.LearningMaterialAssignment)
                                          .Where(LearningMaterialUserAssignmentSpec.ByUserId(request.UserId))
                                          .Where(LearningMaterialUserAssignmentSpec.ByCourseId(request.CourseId))
                                          .Where(LearningMaterialUserAssignmentSpec.NotCompleted())
                                          .ToListAsync(cancellationToken);

                return assignments/*.Where(_ => _contentTreePathResolver.EvalRoot(_.LearningMaterialAssignment.ContentTreeNodeId, courseStructure) == request.RootContentNodeId)*/
                                  .Select(_ =>
                                  {
                                      var fullPath = _contentTreePathResolver.ResolveFullPath(_.LearningMaterialAssignment.ContentTreeNodeId,
                                          courseStructure);
                                      var nameLines = courseTemplate == null
                                          ? fullPath.OrderBy(x => x.Index).Select(x => x.Value).ToList()
                                          : _templateService.ApplyTemplate(courseTemplate.Template.Value,
                                              fullPath.Select(x => new TemplateValue { Index = x.Index, Name = x.Value })
                                                  .ToList()).Split('\n').ToList();
                                      return new ActiveLearningMaterialAssignment
                                      {
                                          LearningMaterialUserAssignmentId = _.LearningMaterialUserAssignmentId,
                                          NodeId = _.LearningMaterialAssignment.ContentTreeNodeId,
                                          Path = _contentTreePathResolver
                                              .ResolvePath(_.LearningMaterialAssignment.ContentTreeNodeId,
                                                  courseStructure)
                                              .ToImmutableList(),
                                          NameLines = nameLines,
                                          AssignedAt = _.AssignedAtUtc,
                                          DueAt = _.DueAtUtc,
                                      };
                                  })
                                  .ToImmutableList();
            }
        }
    }
}

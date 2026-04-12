using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Quizzes.Services;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.CreateQuizFromTemplate
{
    [PublicAPI]
    public class CreateQuizFromTemplateCommand : ICommand<CreateQuizFromTemplateCommand.Result>
    {
        public Guid TemplateId { get; set; }
        public Guid SchoolClassId { get; set; }
        public Guid TeacherId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizAssignmentId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateQuizFromTemplateCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TemplateId).ExistsInTable(Db.QuizTemplates);
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateQuizFromTemplateCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateQuizFromTemplateCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return user.IsTeacherAccountOwner(request.TeacherId);
            }
        }


        internal class Handler : CommandHandlerBase<CreateQuizFromTemplateCommand, Result>
        {
            private readonly IQuestionPoolService _questionPoolService;

            public Handler(IDb db, IQuestionPoolService questionPoolService) : base(db)
            {
                _questionPoolService = questionPoolService;
            }

            public override async Task<Result> Handle(CreateQuizFromTemplateCommand request, CancellationToken cancellationToken)
            {
                var template = await Db.QuizTemplates
                    .Where(t => t.TemplateId == request.TemplateId)
                    .GetSingleAsync(cancellationToken);

                var contentTreeNodeIds = JsonSerializer.Deserialize<List<Guid>>(template.ContentTreeNodeIdsJson);

                var questions = await _questionPoolService.GetPooledQuestionsAsync(
                    template.CourseId,
                    contentTreeNodeIds,
                    template.QuestionCount,
                    request.SchoolClassId);

                var quizEntity = new QuizEntity
                {
                    QuizId = Guid.NewGuid(),
                    ContentTreeId = contentTreeNodeIds.First(),
                    Type = QuizType.TeacherCustom,
                    Name = template.Name,
                    QuestionCount = questions.Count,
                    CreatedByUserId = request.TeacherId,
                };
                await Db.AddAsync(quizEntity, cancellationToken);

                var quizQuestions = questions.Select((q, i) => new QuizQuestionEntity
                {
                    QuizId = quizEntity.QuizId,
                    QuestionId = q.QuestionId,
                    Order = i + 1,
                }).ToList();
                await Db.QuizQuestions.AddRangeAsync(quizQuestions, cancellationToken);

                var studentIds = await Db.SchoolClassStudents
                    .Where(scs => scs.SchoolClassId == request.SchoolClassId)
                    .Select(scs => scs.StudentId)
                    .ToListAsync(cancellationToken);

                var now = DateTime.UtcNow;
                var nowLocal = DateTime.Now;
                var dueUtc = now.AddDays(7);
                var dueLocal = nowLocal.AddDays(7);

                var quizAssignmentEntity = new QuizAssignmentEntity
                {
                    QuizAssignmentId = Guid.NewGuid(),
                    QuizId = quizEntity.QuizId,
                    SchoolClassId = request.SchoolClassId,
                    AssignedByTeacherId = request.TeacherId,
                    DueAtUtc = dueUtc,
                    DueAtLocal = dueLocal,
                    AssignedAtUtc = now,
                    AssignedAtLocal = nowLocal,
                    QuizStudentAssignments = studentIds.Select(studentId => new QuizStudentAssignmentEntity
                    {
                        QuizStudentAssignmentId = Guid.NewGuid(),
                        AssignedToUserId = studentId,
                        AttemptNumber = 1,
                        DueAtUtc = dueUtc,
                        DueAtLocal = dueLocal,
                        AssignedAtUtc = now,
                        AssignedAtLocal = nowLocal,
                    }).ToList(),
                };
                await Db.QuizAssignments.AddAsync(quizAssignmentEntity, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizAssignmentId = quizAssignmentEntity.QuizAssignmentId };
            }
        }
    }
}

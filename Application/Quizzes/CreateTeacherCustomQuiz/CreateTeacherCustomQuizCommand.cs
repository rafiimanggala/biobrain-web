using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Quizzes.Services;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.CreateTeacherCustomQuiz
{
    [PublicAPI]
    public class CreateTeacherCustomQuizCommand : ICommand<CreateTeacherCustomQuizCommand.Result>
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public List<Guid> ContentTreeNodeIds { get; set; }
        public int QuestionCount { get; set; }
        public Guid SchoolClassId { get; set; }
        public bool SaveAsTemplate { get; set; }
        public Guid TeacherId { get; set; }
        public List<Guid> StudentIds { get; set; } = new();
        public DateTime? DueDateUtc { get; set; }
        public DateTime? DueDateLocal { get; set; }
        public bool HintsEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;


        [PublicAPI]
        public class Result
        {
            public Guid QuizAssignmentId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateTeacherCustomQuizCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Name).NotEmpty();
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.ContentTreeNodeIds).NotEmpty();
                RuleFor(_ => _.QuestionCount).GreaterThan(0);
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateTeacherCustomQuizCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService)
                => _db = db;

            protected override bool CanExecute(CreateTeacherCustomQuizCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return user.IsTeacherAccountOwner(request.TeacherId);
            }
        }


        internal class Handler : CommandHandlerBase<CreateTeacherCustomQuizCommand, Result>
        {
            private readonly IQuestionPoolService _questionPoolService;

            public Handler(IDb db, IQuestionPoolService questionPoolService) : base(db)
            {
                _questionPoolService = questionPoolService;
            }

            public override async Task<Result> Handle(CreateTeacherCustomQuizCommand request, CancellationToken cancellationToken)
            {
                var questions = await _questionPoolService.GetPooledQuestionsAsync(
                    request.CourseId,
                    request.ContentTreeNodeIds,
                    request.QuestionCount,
                    request.SchoolClassId);

                var quizEntity = new QuizEntity
                {
                    QuizId = Guid.NewGuid(),
                    ContentTreeId = request.ContentTreeNodeIds.First(),
                    Type = QuizType.TeacherCustom,
                    Name = request.Name,
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

                var studentIds = request.StudentIds != null && request.StudentIds.Count > 0
                    ? request.StudentIds
                    : await Db.SchoolClassStudents
                        .Where(scs => scs.SchoolClassId == request.SchoolClassId)
                        .Select(scs => scs.StudentId)
                        .ToListAsync(cancellationToken);

                var now = DateTime.UtcNow;
                var nowLocal = DateTime.Now;
                var dueUtc = request.DueDateUtc ?? now.AddDays(7);
                var dueLocal = request.DueDateLocal ?? nowLocal.AddDays(7);

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
                    HintsEnabled = request.HintsEnabled,
                    SoundEnabled = request.SoundEnabled,
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

                if (request.SaveAsTemplate)
                {
                    var templateEntity = new QuizTemplateEntity
                    {
                        TemplateId = Guid.NewGuid(),
                        Name = request.Name,
                        CreatedByTeacherId = request.TeacherId,
                        CourseId = request.CourseId,
                        ContentTreeNodeIdsJson = JsonSerializer.Serialize(request.ContentTreeNodeIds),
                        QuestionCount = request.QuestionCount,
                        HintsEnabled = request.HintsEnabled,
                        SoundEnabled = request.SoundEnabled,
                    };
                    await Db.AddAsync(templateEntity, cancellationToken);
                }

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizAssignmentId = quizAssignmentEntity.QuizAssignmentId };
            }
        }
    }
}

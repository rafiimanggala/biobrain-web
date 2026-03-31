using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Biobrain.Application.Quizzes.CreateStudentCustomQuiz
{
    [PublicAPI]
    public class CreateStudentCustomQuizCommand : ICommand<CreateStudentCustomQuizCommand.Result>
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public List<Guid> ContentTreeNodeIds { get; set; }
        public int QuestionCount { get; set; }
        public Guid UserId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizStudentAssignmentId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateStudentCustomQuizCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Name).NotEmpty();
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.ContentTreeNodeIds).NotEmpty();
                RuleFor(_ => _.QuestionCount).GreaterThan(0);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateStudentCustomQuizCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateStudentCustomQuizCommand request, IUserSecurityInfo user)
                => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<CreateStudentCustomQuizCommand, Result>
        {
            private readonly IQuestionPoolService _questionPoolService;

            public Handler(IDb db, IQuestionPoolService questionPoolService) : base(db)
            {
                _questionPoolService = questionPoolService;
            }

            public override async Task<Result> Handle(CreateStudentCustomQuizCommand request, CancellationToken cancellationToken)
            {
                var questions = await _questionPoolService.GetPooledQuestionsAsync(
                    request.CourseId,
                    request.ContentTreeNodeIds,
                    request.QuestionCount);

                var quizEntity = new QuizEntity
                {
                    QuizId = Guid.NewGuid(),
                    ContentTreeId = request.ContentTreeNodeIds.First(),
                    Type = QuizType.StudentCustom,
                    Name = request.Name,
                    QuestionCount = questions.Count,
                    CreatedByUserId = request.UserId,
                };
                await Db.AddAsync(quizEntity, cancellationToken);

                var quizQuestions = questions.Select((q, i) => new QuizQuestionEntity
                {
                    QuizId = quizEntity.QuizId,
                    QuestionId = q.QuestionId,
                    Order = i + 1,
                }).ToList();
                await Db.QuizQuestions.AddRangeAsync(quizQuestions, cancellationToken);

                var quizAssignmentEntity = new QuizAssignmentEntity
                {
                    QuizAssignmentId = Guid.NewGuid(),
                    QuizId = quizEntity.QuizId,
                    SchoolClassId = null,
                    AssignedByTeacherId = null,
                };
                await Db.AddAsync(quizAssignmentEntity, cancellationToken);

                var quizStudentAssignmentEntity = new QuizStudentAssignmentEntity
                {
                    QuizStudentAssignmentId = Guid.NewGuid(),
                    QuizAssignmentId = quizAssignmentEntity.QuizAssignmentId,
                    AssignedToUserId = request.UserId,
                    AttemptNumber = 1,
                };
                await Db.AddAsync(quizStudentAssignmentEntity, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizStudentAssignmentId = quizStudentAssignmentEntity.QuizStudentAssignmentId };
            }
        }
    }
}

using System;
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
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GenerateTopicQuiz
{
    [PublicAPI]
    public class GenerateTopicQuizCommand : ICommand<GenerateTopicQuizCommand.Result>
    {
        public Guid ContentTreeNodeId { get; set; }
        public int QuestionCount { get; set; }
        public Guid UserId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizStudentAssignmentId { get; set; }
        }


        internal class Validator : ValidatorBase<GenerateTopicQuizCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.ContentTreeNodeId).ExistsInTable(Db.ContentTree);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.QuestionCount)
                    .Must(c => c == 20 || c == 30 || c == 40 || c == 60)
                    .WithMessage("Question count must be 20, 30, 40, or 60.");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GenerateTopicQuizCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GenerateTopicQuizCommand request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<GenerateTopicQuizCommand, Result>
        {
            private readonly IQuestionPoolService _questionPoolService;

            public Handler(IDb db, IQuestionPoolService questionPoolService) : base(db)
                => _questionPoolService = questionPoolService;

            public override async Task<Result> Handle(GenerateTopicQuizCommand request, CancellationToken cancellationToken)
            {
                var contentNode = await Db.ContentTree
                    .Where(ct => ct.NodeId == request.ContentTreeNodeId)
                    .FirstAsync(cancellationToken);

                var pooledQuestions = await _questionPoolService.GetPooledQuestionsAsync(
                    contentNode.CourseId,
                    new[] { request.ContentTreeNodeId },
                    request.QuestionCount);

                var quizEntity = new QuizEntity
                {
                    ContentTreeId = request.ContentTreeNodeId,
                    Type = QuizType.EndOfTopic,
                    QuestionCount = request.QuestionCount,
                    Name = $"Topic Quiz: {contentNode.Name}",
                    CreatedByUserId = request.UserId,
                };
                await Db.AddAsync(quizEntity, cancellationToken);

                var order = 1;
                foreach (var question in pooledQuestions)
                {
                    var quizQuestion = new QuizQuestionEntity
                    {
                        QuizId = quizEntity.QuizId,
                        QuestionId = question.QuestionId,
                        Order = order++,
                    };
                    await Db.AddAsync(quizQuestion, cancellationToken);
                }

                var quizAssignment = new QuizAssignmentEntity
                {
                    SchoolClassId = null,
                    AssignedByTeacherId = null,
                    QuizId = quizEntity.QuizId,
                };
                await Db.AddAsync(quizAssignment, cancellationToken);

                var studentAssignment = new QuizStudentAssignmentEntity
                {
                    QuizAssignmentId = quizAssignment.QuizAssignmentId,
                    AssignedToUserId = request.UserId,
                    AttemptNumber = 1,
                };
                await Db.AddAsync(studentAssignment, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizStudentAssignmentId = studentAssignment.QuizStudentAssignmentId };
            }
        }
    }
}

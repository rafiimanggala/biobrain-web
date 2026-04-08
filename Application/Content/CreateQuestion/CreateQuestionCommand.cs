using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using DataAccessLayer.WebAppEntities;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.CreateQuestion
{
    [PublicAPI]
    public sealed class CreateQuestionCommand : ICommand<CreateQuestionCommand.Result>
    {
        public Guid CourseId { get; set; }
        public long QuestionTypeCode { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public string FeedBack { get; set; }
        public List<AnswerInput> Answers { get; set; } = new();

        // Optional: attach to a content tree node (creates/uses Quiz on that node)
        public Guid? NodeId { get; set; }

        public sealed class AnswerInput
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
            public bool CaseSensitive { get; set; }
            public int Score { get; set; }
        }

        [PublicAPI]
        public sealed class Result
        {
            public Guid QuestionId { get; set; }
        }

        internal class Validator : ValidatorBase<CreateQuestionCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.QuestionTypeCode).GreaterThan(0);
                RuleFor(_ => _.Text).NotEmpty();
                RuleFor(_ => _.Answers).NotNull();
                RuleFor(_ => _.Answers)
                    .Must(a => a != null && a.Count > 0)
                    .WithMessage("At least one answer is required.");
                RuleFor(_ => _.Answers)
                    .Must(a => a == null || a.Any(x => x.IsCorrect))
                    .WithMessage("At least one answer must be marked correct.");

                When(_ => _.NodeId.HasValue, () =>
                {
                    RuleFor(_ => _.NodeId.Value)
                        .ExistsInTable(Db.ContentTree);

                    RuleFor(_ => _.NodeId.Value)
                        .MustAsync(async (command, nodeId, _) => (
                                await Db.ContentTree
                                    .Where(x => x.NodeId == nodeId)
                                    .Include(x => x.ContentTreeMeta)
                                    .FirstAsync())
                            .ContentTreeMeta.CouldAddContent)
                        .WithMessage("Can't attach questions to this node.");
                });
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<CreateQuestionCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(CreateQuestionCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<CreateQuestionCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
            {
                await using var transaction = await Db.BeginTransactionAsync(cancellationToken);

                var question = new QuestionEntity
                {
                    CourseId = request.CourseId,
                    QuestionTypeCode = request.QuestionTypeCode,
                    Header = request.Header ?? "",
                    Text = request.Text ?? "",
                    Hint = request.Hint ?? "",
                    FeedBack = request.FeedBack ?? ""
                };
                var qEntry = await Db.AddAsync(question, cancellationToken);
                var questionId = ((QuestionEntity)qEntry.Entity).QuestionId;

                var answerOrder = 0;
                foreach (var answer in request.Answers)
                {
                    await Db.AddAsync(new AnswerEntity
                    {
                        QuestionId = questionId,
                        CourseId = request.CourseId,
                        Text = answer.Text ?? "",
                        IsCorrect = answer.IsCorrect,
                        CaseSensitive = answer.CaseSensitive,
                        AnswerOrder = answerOrder++,
                        Score = answer.IsCorrect ? Math.Max(answer.Score, 1) : answer.Score
                    }, cancellationToken);
                }

                if (request.NodeId.HasValue)
                {
                    var quiz = await Db.Quizzes
                                   .Where(x => x.ContentTreeId == request.NodeId.Value)
                                   .FirstOrDefaultAsync(cancellationToken)
                               ?? (await Db.Quizzes.AddAsync(new QuizEntity
                               {
                                   ContentTreeId = request.NodeId.Value,
                                   CreatedAt = DateTime.UtcNow,
                                   UpdatedAt = DateTime.UtcNow
                               }, cancellationToken)).Entity;

                    var maxOrder = await Db.QuizQuestions
                        .Where(x => x.QuizId == quiz.QuizId)
                        .Select(x => (int?)x.Order)
                        .MaxAsync(cancellationToken) ?? -1;

                    await Db.AddAsync(new QuizQuestionEntity
                    {
                        QuizId = quiz.QuizId,
                        QuestionId = questionId,
                        Order = maxOrder + 1
                    }, cancellationToken);
                }

                await Db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new Result { QuestionId = questionId };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using DataAccessLayer.WebAppEntities;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.UpdateQuestion
{
    [PublicAPI]
    public sealed class UpdateQuestionCommand : ICommand<UpdateQuestionCommand.Result>
    {
        public Guid QuestionId { get; set; }
        public long QuestionTypeCode { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public string FeedBack { get; set; }
        public List<AnswerInput> Answers { get; set; } = new();

        public sealed class AnswerInput
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
            public bool CaseSensitive { get; set; }
            public int Score { get; set; }
        }

        [PublicAPI]
        public sealed class Result;

        internal class Validator : ValidatorBase<UpdateQuestionCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuestionId).ExistsInTable(Db.Questions);
                RuleFor(_ => _.QuestionTypeCode).GreaterThan(0);
                RuleFor(_ => _.Text).NotEmpty();
                RuleFor(_ => _.Answers)
                    .Must(a => a != null && a.Count > 0)
                    .WithMessage("At least one answer is required.");
                RuleFor(_ => _.Answers)
                    .Must(a => a == null || a.Any(x => x.IsCorrect))
                    .WithMessage("At least one answer must be marked correct.");
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UpdateQuestionCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(UpdateQuestionCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<UpdateQuestionCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
            {
                await using var transaction = await Db.BeginTransactionAsync(cancellationToken);

                var question = await Db.Questions
                    .Where(x => x.QuestionId == request.QuestionId && x.DeletedAt == null)
                    .FirstAsync(cancellationToken);

                question.QuestionTypeCode = request.QuestionTypeCode;
                question.Header = request.Header ?? "";
                question.Text = request.Text ?? "";
                question.Hint = request.Hint ?? "";
                question.FeedBack = request.FeedBack ?? "";

                // Replace answers: delete old, insert new
                var oldAnswers = await Db.Answers
                    .Where(x => x.QuestionId == request.QuestionId)
                    .ToListAsync(cancellationToken);
                Db.Answers.RemoveRange(oldAnswers);

                var answerOrder = 0;
                foreach (var answer in request.Answers)
                {
                    await Db.AddAsync(new AnswerEntity
                    {
                        QuestionId = request.QuestionId,
                        CourseId = question.CourseId,
                        Text = answer.Text ?? "",
                        IsCorrect = answer.IsCorrect,
                        CaseSensitive = answer.CaseSensitive,
                        AnswerOrder = answerOrder++,
                        Score = answer.IsCorrect ? Math.Max(answer.Score, 1) : answer.Score
                    }, cancellationToken);
                }

                await Db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new Result();
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.UpdateQuizSettings
{
    [PublicAPI]
    public sealed class UpdateQuizSettingsCommand : ICommand<UpdateQuizSettingsCommand.Result>
    {
        public Guid QuizId { get; set; }
        public string Name { get; set; }
        public int? QuestionCount { get; set; }

        [PublicAPI]
        public sealed class Result
        {
            public Guid QuizId { get; set; }
        }

        internal class Validator : ValidatorBase<UpdateQuizSettingsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
                RuleFor(_ => _.QuestionCount)
                    .Must(c => !c.HasValue || c.Value > 0)
                    .WithMessage("Question count must be positive.");
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UpdateQuizSettingsCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(UpdateQuizSettingsCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<UpdateQuizSettingsCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(UpdateQuizSettingsCommand request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes
                    .Where(q => q.QuizId == request.QuizId)
                    .FirstAsync(cancellationToken);

                quiz.Name = request.Name ?? quiz.Name;
                quiz.QuestionCount = request.QuestionCount;
                quiz.UpdatedAt = DateTime.UtcNow;

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizId = quiz.QuizId };
            }
        }
    }
}

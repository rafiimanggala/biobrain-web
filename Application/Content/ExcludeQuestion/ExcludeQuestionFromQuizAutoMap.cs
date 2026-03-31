using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Question;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.ExcludeQuestion
{
    [PublicAPI]
    public sealed class ExcludeQuestionFromQuizAutoMap : IQuery<ExcludeQuestionFromQuizAutoMap.Result>
    {
	    public Guid QuizId { get; set; }
	    public Guid QuestionId { get; set; }


        [PublicAPI]
        public record Result;

        internal class Validator : ValidatorBase<ExcludeQuestionFromQuizAutoMap>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
		        RuleFor(_ => _.QuestionId).ExistsInTable(Db.Questions);
	        }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<ExcludeQuestionFromQuizAutoMap>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(ExcludeQuestionFromQuizAutoMap request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<ExcludeQuestionFromQuizAutoMap, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(ExcludeQuestionFromQuizAutoMap request, CancellationToken cancellationToken)
            {
                // Get existing page or create new
	            var quizQuestion = await Db.QuizQuestions.Where(_ => _.QuizId == request.QuizId && _.QuestionId == request.QuestionId).SingleAsync(cancellationToken);
                await Db.QuizExcludedQuestions.AddAsync(new QuizExcludedQuestionEntity
                {
                    QuizId = request.QuizId,
                    QuestionId = request.QuestionId,
                    Order = quizQuestion.Order
                }, cancellationToken);
                Db.QuizQuestions.Remove(quizQuestion);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
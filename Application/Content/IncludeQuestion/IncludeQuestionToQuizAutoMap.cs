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

namespace Biobrain.Application.Content.IncludeQuestion
{
    [PublicAPI]
    public sealed class IncludeQuestionToQuizAutoMap : IQuery<IncludeQuestionToQuizAutoMap.Result>
    {
	    public Guid QuizId { get; set; }
	    public Guid QuestionId { get; set; }


        [PublicAPI]
        public record Result;

        internal class Validator : ValidatorBase<IncludeQuestionToQuizAutoMap>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
		        RuleFor(_ => _.QuestionId).ExistsInTable(Db.Questions);
	        }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<IncludeQuestionToQuizAutoMap>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(IncludeQuestionToQuizAutoMap request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<IncludeQuestionToQuizAutoMap, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(IncludeQuestionToQuizAutoMap request, CancellationToken cancellationToken)
            {
                // Get existing page or create new
	            var quizExcludedQuestion = await Db.QuizExcludedQuestions.Where(_ => _.QuizId == request.QuizId && _.QuestionId == request.QuestionId).SingleAsync(cancellationToken);
                await Db.QuizQuestions.AddAsync(new QuizQuestionEntity
                {
                    QuizId = request.QuizId,
                    QuestionId = request.QuestionId,
                    Order = quizExcludedQuestion.Order
                }, cancellationToken);
                Db.QuizExcludedQuestions.Remove(quizExcludedQuestion);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
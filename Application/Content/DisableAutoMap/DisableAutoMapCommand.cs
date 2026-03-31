using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.DisableAutoMap
{
    [PublicAPI]
    public sealed class DisableAutoMapCommand : IQuery<DisableAutoMapCommand.Result>
    {
	    public Guid QuizId { get; set; }


        [PublicAPI]
        public record Result;

        internal class Validator : ValidatorBase<DisableAutoMapCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
	        }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<DisableAutoMapCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(DisableAutoMapCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<DisableAutoMapCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(DisableAutoMapCommand request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes.Where(QuizSpec.ById(request.QuizId)).SingleAsync(cancellationToken);
                quiz.AutoMapQuizId = null;
                var excludedQuestions = await Db.QuizExcludedQuestions.Where(_ => _.QuizId == request.QuizId)
                    .ToListAsync(cancellationToken);
                Db.QuizExcludedQuestions.RemoveRange(excludedQuestions);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
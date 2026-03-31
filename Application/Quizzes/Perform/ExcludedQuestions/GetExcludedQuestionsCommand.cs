using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.Perform.ExcludedQuestions
{
    [PublicAPI]
    public sealed class GetExcludedQuestionsCommand : ICommand<GetExcludedQuestionsCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public Guid QuizId { get; init; }


        [PublicAPI]
        public record Result
        {
            public ImmutableList<Guid> QuestionIds { get; init; }
        }


        internal sealed class Validator : ValidatorBase<GetExcludedQuestionsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetExcludedQuestionsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetExcludedQuestionsCommand request, IUserSecurityInfo user) => true;
        }


        internal sealed class Handler : CommandHandlerBase<GetExcludedQuestionsCommand, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetExcludedQuestionsCommand request, CancellationToken cancellationToken)
            {
                var excluded = await Db.ExcludedQuestions.Where(_ => _.QuizId == request.QuizId && _.SchoolClassId == request.SchoolClassId).ToListAsync(cancellationToken);
                
                return new Result
                {
                    QuestionIds = excluded.Select(_ => _.QuestionId).ToImmutableList(),
                };
            }

           
        }
    }
}

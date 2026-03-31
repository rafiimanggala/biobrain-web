using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.Perform.ExcludedQuestions
{
    [PublicAPI]
    public sealed class AddExcludedQuestionCommand : ICommand<AddExcludedQuestionCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public Guid QuizId { get; init; }
        public Guid QuestionId { get; init; }
        public bool IsExcluded { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<AddExcludedQuestionCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
                RuleFor(_ => _.QuestionId).ExistsInTable(Db.Questions);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<AddExcludedQuestionCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(AddExcludedQuestionCommand request, IUserSecurityInfo user) => true;
        }


        internal sealed class Handler : CommandHandlerBase<AddExcludedQuestionCommand, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(AddExcludedQuestionCommand request, CancellationToken cancellationToken)
            {
                if (request.IsExcluded)
                {
                    await Db.ExcludedQuestions.AddAsync(new ExcludedQuestionEntity
                    {
                        SchoolClassId = request.SchoolClassId,
                        QuizId = request.QuizId,
                        QuestionId = request.QuestionId,
                    }, cancellationToken);
                }
                else
                {
                    var entity = await Db.ExcludedQuestions.Where(_ => _.SchoolClassId == request.SchoolClassId && _.QuizId == request.QuizId && _.QuestionId == request.QuestionId).FirstOrDefaultAsync(cancellationToken);
                    if(entity == null) return new Result();

                    Db.Remove(entity);
                }

                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                {
                };
            }

           
        }
    }
}

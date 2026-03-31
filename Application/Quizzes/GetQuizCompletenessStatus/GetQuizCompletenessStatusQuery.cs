using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetQuizCompletenessStatus
{
    [PublicAPI]
    public class GetQuizFullnessStatusQuery : ICommand<GetQuizFullnessStatusQuery.Result>
    {
        public Guid QuizId { get; set; }


        [PublicAPI]
        public class Result
        {
            public bool IsQuizFull { get; set; }
        }


        internal class Validator : ValidatorBase<GetQuizFullnessStatusQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizFullnessStatusQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GetQuizFullnessStatusQuery request, IUserSecurityInfo user) => true;
        }


        internal class Handler : CommandHandlerBase<GetQuizFullnessStatusQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetQuizFullnessStatusQuery request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes.AsNoTracking()
                    .Where(_ => _.QuizId == request.QuizId)
                    .Include(_ => _.QuizQuestions).ThenInclude(_ => _.Question)//.ThenInclude(_ => _.QuizQuestions).ThenInclude(_=> _.Quiz)
                    .Include(_ => _.ContentTreeNode).ThenInclude(_ => _.Course)
                    .FirstOrDefaultAsync(cancellationToken);
                var baseQuestion = quiz.QuizQuestions.FirstOrDefault()?.Question;
                var baseQuiz = await Db.Quizzes.AsNoTracking()
                    .Include(_ => _.ContentTreeNode).ThenInclude(_ => _.Course)
                    .Include(_ => _.QuizQuestions)
                    .Where(_ => _.ContentTreeNode.Course.IsBase && _.QuizQuestions.Any(_ => _.QuestionId == baseQuestion.QuestionId))
                    .FirstOrDefaultAsync(cancellationToken);
                if (baseQuestion == null ||  baseQuiz == null) return new Result { IsQuizFull = true };
                if (quiz.QuizQuestions.Count == baseQuiz.QuizQuestions.Count) return new Result { IsQuizFull = true };

                return new Result { IsQuizFull = false };
            }
        }
    }
}

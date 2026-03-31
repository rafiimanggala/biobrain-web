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

namespace Biobrain.Application.Content.GetAutoMapOptions
{
    [PublicAPI]
    public sealed class GetAutoMapOptionsQuery : IQuery<GetAutoMapOptionsQuery.Result>
    {
	    public Guid QuizId { get; set; }


        [PublicAPI]
        public record Result
        {
	        public Guid? BaseQuizId { get; init; }
	        public Guid? BaseCourseId { get; init; }
        }

        internal class Validator : ValidatorBase<GetAutoMapOptionsQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetAutoMapOptionsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetAutoMapOptionsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetAutoMapOptionsQuery, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(GetAutoMapOptionsQuery request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes.AsNoTracking()
                    .Include(_ => _.AutoMapQuiz).ThenInclude(_ => _.ContentTreeNode)
                    .Where(QuizSpec.ById(request.QuizId))
                    .SingleAsync(cancellationToken);

                if (quiz.AutoMapQuiz == null) return new Result { BaseCourseId = null, BaseQuizId = null };

                return new Result
                {
                    BaseCourseId = quiz.AutoMapQuiz.ContentTreeNode.CourseId,
                    BaseQuizId = quiz.AutoMapQuizId
                };
            }
        }
    }
}
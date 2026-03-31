using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.QuizAutoMap;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.UpdateAutoMapOptions
{
    [PublicAPI]
    public sealed class UpdateAutoMapOptionsCommand : IQuery<UpdateAutoMapOptionsCommand.Result>
    {
	    public Guid QuizId { get; set; }
        public Guid BaseQuizId { get; set; }
        public Guid ParentNodeId { get; set; }


        [PublicAPI]
        public record Result
        {
        }

        internal class Validator : ValidatorBase<UpdateAutoMapOptionsCommand>
        {
	        public Validator(IDb db) : base(db)
	        {
		        //RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes, QuizSpec.ById);
		        RuleFor(_ => _.BaseQuizId).ExistsInTable(Db.Quizzes, QuizSpec.ById);
		        RuleFor(_ => _.ParentNodeId).ExistsInTable(Db.ContentTree, ContentTreeSpec.ById);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UpdateAutoMapOptionsCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(UpdateAutoMapOptionsCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<UpdateAutoMapOptionsCommand, Result>
        {
            private readonly IQuizAutoMapService _quizAutoMapService;

            public Handler(IDb db, IQuizAutoMapService quizAutoMapService) : base(db) => _quizAutoMapService = quizAutoMapService;

            public override async Task<Result> Handle(UpdateAutoMapOptionsCommand request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes.Where(QuizSpec.ById(request.QuizId)).FirstOrDefaultAsync(cancellationToken);
                if (quiz == null)
                {
                    quiz = new QuizEntity
                    {
                        QuizId = request.QuizId,
                        ContentTreeId = request.ParentNodeId
                    };
                    await Db.Quizzes.AddAsync(quiz, cancellationToken);
                }

                quiz.AutoMapQuizId = request.BaseQuizId;

                await Db.SaveChangesAsync(cancellationToken);

                await _quizAutoMapService.MapQuiz(request.QuizId);

                return new Result();
            }
        }
    }
}
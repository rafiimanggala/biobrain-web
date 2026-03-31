using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Quizzes.EnsureQuizResultForAssignment
{
    [PublicAPI]
    public sealed class EnsureQuizResultForAssignmentCommand : ICommand<EnsureQuizResultForAssignmentCommand.Result>
    {
        public Guid UserId { get; init; }
        public Guid QuizStudentAssignmentId { get; init; }


        [PublicAPI]
        public record Result(Guid QuizResultId);


        internal sealed class Validator : ValidatorBase<EnsureQuizResultForAssignmentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.QuizStudentAssignmentId).ExistsInTable(Db.QuizStudentAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<EnsureQuizResultForAssignmentCommand>
        {
            private readonly ISessionContext _sessionContext;
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext, IDb db)
                : base(securityService)
            {
                _sessionContext = sessionContext;
                _db = db;
            }

            protected override bool CanExecute(EnsureQuizResultForAssignmentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _db.QuizStudentAssignments
                          .Where(QuizStudentAssignmentSpec.ById(request.QuizStudentAssignmentId))
                          .Where(QuizStudentAssignmentSpec.ForUser(request.UserId))
                          .Any();
            }
        }


        internal sealed class Handler : CommandHandlerBase<EnsureQuizResultForAssignmentCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(EnsureQuizResultForAssignmentCommand request, CancellationToken cancellationToken)
            {
                var quizResult = await Db.QuizResults
                                         .AsNoTracking()
                                         .Where(QuizResultSpec.ByStudentAssignmentId(request.QuizStudentAssignmentId))
                                         .SingleOrDefaultAsync(cancellationToken) ??
                                 await CreateResult(request, cancellationToken);

                return new Result(quizResult.QuizStudentAssignmentId);
            }

            private async Task<QuizResultEntity> CreateResult(EnsureQuizResultForAssignmentCommand request, CancellationToken cancellationToken)
            {
                var assignment = await Db.QuizStudentAssignments
                                         .AsNoTracking()
                                         .Include(_ => _.QuizAssignment)
                                         .Where(QuizStudentAssignmentSpec.ById(request.QuizStudentAssignmentId))
                                         .SingleAsync(cancellationToken);

                var quizResult = new QuizResultEntity
                                 {
                                     QuizStudentAssignmentId = assignment.QuizStudentAssignmentId,
                                     StaredAt = DateTime.UtcNow
                                 };

                await Db.QuizResults.AddAsync(quizResult, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                return quizResult;
            }
        }
    }
}

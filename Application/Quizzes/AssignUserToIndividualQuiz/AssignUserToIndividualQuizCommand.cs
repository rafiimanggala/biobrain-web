using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;

namespace Biobrain.Application.Quizzes.AssignUserToIndividualQuiz
{
    [PublicAPI]
    public class AssignUserToIndividualQuizCommand : ICommand<AssignUserToIndividualQuizCommand.Result>
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizResultId { get; set; }
        }


        internal class Validator : ValidatorBase<AssignUserToIndividualQuizCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<AssignUserToIndividualQuizCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(AssignUserToIndividualQuizCommand request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<AssignUserToIndividualQuizCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(AssignUserToIndividualQuizCommand request, CancellationToken cancellationToken)
            {
                var quizAssignmentEntity = new QuizAssignmentEntity
                                           {
                                               SchoolClassId = null,
                                               AssignedByTeacherId = null,
                                               QuizId = request.QuizId,
                                           };
                await Db.AddAsync(quizAssignmentEntity, cancellationToken);

                var quizStudentAssignmentEntity = new QuizStudentAssignmentEntity
                                                  {
                                                      QuizAssignmentId = quizAssignmentEntity.QuizAssignmentId,
                                                      AssignedToUserId = request.UserId,
                                                      AttemptNumber = 1,
                                                  };
                await Db.AddAsync(quizStudentAssignmentEntity, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result {QuizResultId = quizStudentAssignmentEntity.QuizStudentAssignmentId};
            }
        }
    }
}

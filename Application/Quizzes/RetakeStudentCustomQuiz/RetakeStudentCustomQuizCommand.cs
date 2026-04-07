using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Quiz;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.RetakeStudentCustomQuiz
{
    [PublicAPI]
    public class RetakeStudentCustomQuizCommand : ICommand<RetakeStudentCustomQuizCommand.Result>
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizStudentAssignmentId { get; set; }
        }


        internal class Validator : ValidatorBase<RetakeStudentCustomQuizCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizId).ExistsInTable(Db.Quizzes);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<RetakeStudentCustomQuizCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(RetakeStudentCustomQuizCommand request, IUserSecurityInfo user)
                => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<RetakeStudentCustomQuizCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(RetakeStudentCustomQuizCommand request, CancellationToken cancellationToken)
            {
                var quiz = await Db.Quizzes
                    .Where(_ => _.QuizId == request.QuizId
                                && _.Type == QuizType.StudentCustom
                                && _.CreatedByUserId == request.UserId)
                    .SingleAsync(cancellationToken);

                var nextAttempt = await Db.QuizStudentAssignments
                    .Where(_ => _.QuizAssignment.QuizId == quiz.QuizId
                                && _.AssignedToUserId == request.UserId)
                    .CountAsync(cancellationToken) + 1;

                var quizAssignment = new QuizAssignmentEntity
                {
                    QuizAssignmentId = Guid.NewGuid(),
                    QuizId = quiz.QuizId,
                    SchoolClassId = null,
                    AssignedByTeacherId = null,
                };
                await Db.AddAsync(quizAssignment, cancellationToken);

                var quizStudentAssignment = new QuizStudentAssignmentEntity
                {
                    QuizStudentAssignmentId = Guid.NewGuid(),
                    QuizAssignmentId = quizAssignment.QuizAssignmentId,
                    AssignedToUserId = request.UserId,
                    AttemptNumber = nextAttempt,
                };
                await Db.AddAsync(quizStudentAssignment, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result { QuizStudentAssignmentId = quizStudentAssignment.QuizStudentAssignmentId };
            }
        }
    }
}

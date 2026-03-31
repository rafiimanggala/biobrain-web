using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetQuizResult
{
    [PublicAPI]
    public class GetQuizResultQuery : ICommand<GetQuizResultQuery.Result>
    {
        public Guid QuizResultId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid QuizResultId { get; set; }
            public Guid QuizId { get; set; }
            public Guid UserId { get; set; }
            public Guid? SchoolClassId { get; init; }
            public Guid? SchoolId { get; init; }
            public string SchoolName { get; init; }
            public double Score { get; set; }
            public List<ResultQuestion> Questions { get; set; } = new();
            public List<Guid> ExcludedQuestions { get; set; } = new();
            public bool HintsEnabled { get; set; } = true;
            public bool SoundEnabled { get; set; } = true;
        }

        public record ResultQuestion(Guid QuizResultId, Guid QuestionId, string Value, bool IsCorrect);


        internal class Validator : ValidatorBase<GetQuizResultQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizResultId).ExistsInTable(Db.QuizStudentAssignments);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizResultQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GetQuizResultQuery request, IUserSecurityInfo user)
            {
                var quizStudentAssignmentEntity = _db.QuizStudentAssignments.Where(_ => _.QuizStudentAssignmentId == request.QuizResultId).GetSingle();
                return user.IsAccountOwner(quizStudentAssignmentEntity.AssignedToUserId);
            }
        }


        internal class Handler : CommandHandlerBase<GetQuizResultQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetQuizResultQuery request, CancellationToken cancellationToken)
            {
                var quizStudentAssignmentEntity = await Db.QuizStudentAssignments
                                                          .Include(_ => _.QuizAssignment).ThenInclude(_ => _.SchoolClass).ThenInclude(_ => _.School)
                                                          .Include(_ => _.Result).ThenInclude(_ => _.Questions)
                                                          .Where(_ => _.QuizStudentAssignmentId == request.QuizResultId)
                                                          .GetSingleAsync(cancellationToken);

                var questions = quizStudentAssignmentEntity.Result?.Questions.OrderBy(_ => _.CreatedAt).Select(_ => new ResultQuestion(_.QuizResultId, _.QuestionId, _.Value, _.IsCorrect)).ToList();
                var excludedQuestions = quizStudentAssignmentEntity.QuizAssignment.SchoolClassId == null 
                    ? new List<ExcludedQuestionEntity>()
                    : await Db.ExcludedQuestions.Where(_ => 
                        _.SchoolClassId == quizStudentAssignmentEntity.QuizAssignment.SchoolClassId
                        && _.QuizId == quizStudentAssignmentEntity.QuizAssignment.QuizId
                        ).ToListAsync(cancellationToken);

                return new Result
                       {
                           QuizResultId = quizStudentAssignmentEntity.QuizStudentAssignmentId,
                           UserId = quizStudentAssignmentEntity.AssignedToUserId,
                           QuizId = quizStudentAssignmentEntity.QuizAssignment.QuizId,
                           SchoolClassId = quizStudentAssignmentEntity.QuizAssignment.SchoolClassId,
                           SchoolId = quizStudentAssignmentEntity.QuizAssignment.SchoolClass?.SchoolId,
                           SchoolName = quizStudentAssignmentEntity.QuizAssignment.SchoolClass?.School.Name,
                           Score = quizStudentAssignmentEntity.Result?.Score ?? 0,
                           Questions = questions ?? new List<ResultQuestion>(),
                           ExcludedQuestions = excludedQuestions.Select(_ => _.QuestionId).ToList(),
                           HintsEnabled = quizStudentAssignmentEntity.QuizAssignment.HintsEnabled,
                           SoundEnabled = quizStudentAssignmentEntity.QuizAssignment.SoundEnabled,
                       };
            }
        }
    }
}

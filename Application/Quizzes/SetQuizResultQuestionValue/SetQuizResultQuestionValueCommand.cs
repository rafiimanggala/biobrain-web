using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Quiz;
using BiobrainWebAPI.Values;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.SetQuizResultQuestionValue
{
    [PublicAPI]
    public class SetQuizResultQuestionValueCommand : ICommand<SetQuizResultQuestionValueCommand.Result>
    {
        public Guid QuizResultId { get; set; }
        public Guid QuestionId { get; set; }
        public string Value { get; set; }
        public DateTime LocalDate { get; set; }
        public bool IsCorrect { get; set; }

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<SetQuizResultQuestionValueCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizResultId).ExistsInTable(Db.QuizStudentAssignments);
                RuleFor(_ => _.QuestionId).ExistsInTable(Db.Questions);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SetQuizResultQuestionValueCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(SetQuizResultQuestionValueCommand request, IUserSecurityInfo user)
            {
                var quizStudentAssignmentEntity = _db.QuizStudentAssignments.Where(_ => _.QuizStudentAssignmentId == request.QuizResultId).GetSingle();
                return user.IsAccountOwner(quizStudentAssignmentEntity.AssignedToUserId);
            }
        }


        internal class Handler : CommandHandlerBase<SetQuizResultQuestionValueCommand, Result>
        {
	        private readonly ISessionContext _sessionContext;
            public Handler(IDb db, ISessionContext sessionContext) : base(db) => _sessionContext = sessionContext;

            public override async Task<Result> Handle(SetQuizResultQuestionValueCommand request, CancellationToken cancellationToken)
            {
                var quizStudentAssignmentEntity = await Db.QuizStudentAssignments
                                                          .Include(_ => _.Result).ThenInclude(_ => _.Questions)
                                                          .Include(_ => _.QuizAssignment).ThenInclude(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                                          .Include(_ => _.QuizAssignment).ThenInclude(_ => _.Quiz).ThenInclude(_ => _.QuizQuestions)
                                                          .Where(_ => _.QuizStudentAssignmentId == request.QuizResultId)
                                                          .GetSingleAsync(cancellationToken);
                var quizResult = await GetOrCreateQuizResult(quizStudentAssignmentEntity, cancellationToken);
                var quizResultQuestion = await GetOrCreateQuestion(request.QuestionId, quizResult, cancellationToken);
                var excludedQuestions = quizStudentAssignmentEntity.QuizAssignment.SchoolClassId == null
                    ? new List<ExcludedQuestionEntity>()
                    : await Db.ExcludedQuestions.Where(_ =>
                        _.SchoolClassId == quizStudentAssignmentEntity.QuizAssignment.SchoolClassId
                        && _.QuizId == quizStudentAssignmentEntity.QuizAssignment.QuizId
                    ).ToListAsync(cancellationToken);

                quizResultQuestion.Value = request.Value;
                quizResultQuestion.IsCorrect = request.IsCorrect;

                var quiz = quizStudentAssignmentEntity.QuizAssignment.Quiz;
                var quizQuestions = await Db.QuizQuestions.Where(_ => _.QuizId == quizStudentAssignmentEntity.QuizAssignment.QuizId).Select(_ => _.QuestionId).ToListAsync(cancellationToken);
                var quizQuestionLimit = quiz.QuestionCount ?? AppSettings.QuizQuestionNumber;
                var questionsCount = Math.Min(quizQuestions.Count(_ => excludedQuestions.All(eq => _ != eq.QuestionId)), quizQuestionLimit);
                var isCompleted = quizResult.Questions.Count >= questionsCount;
                quizResult.CompletedAt = isCompleted ? DateTime.UtcNow : null;
                quizResult.Score = quizQuestions.Count == 0 ? 0 : 100 * quizResult.Questions.Count(_ => _.IsCorrect) / questionsCount;

                // Save streack info for students
                if (isCompleted && _sessionContext.IsUserInRole(Constant.Roles.Student))
	                await UpdateStreak(quizStudentAssignmentEntity.QuizAssignment.Quiz.ContentTreeNode.CourseId,
		                quizStudentAssignmentEntity.AssignedToUserId, request.LocalDate.ToLocalTime(), cancellationToken);
                
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }

            private async Task UpdateStreak(Guid courseId, Guid studentId, DateTime localDate, CancellationToken cancellationToken)
            {
	            var streak = await Db.QuizStreak.Where(_ => _.CourseId == courseId && _.StudentId == studentId)
		            .FirstOrDefaultAsync(cancellationToken);

	            if (streak == null)
	            {
		            Db.QuizStreak.Add(new QuizStreakEntity
		            {
                        CourseId = courseId,
                        StudentId = studentId,
                        WeeksStreak = 0,
                        DaysCount = 1,
                        UpdatedAtLocal = localDate
		            });
                    return;
	            }

                if(localDate.Date == streak.UpdatedAtLocal.Date) return;

	            if (localDate.AddDays(-1).Date == streak.UpdatedAtLocal.Date)
	            {
		            streak.DaysCount++;
		            streak.UpdatedAtLocal = localDate;

                    if ((double)streak.DaysCount % 7 == 0)
		            {
			            streak.WeeksStreak = streak.DaysCount / 7;
		            }

		            Db.Update(streak);
                    return;
	            }

	            if (localDate.AddDays(-1).Date > streak.UpdatedAtLocal.Date)
	            {
		            streak.DaysCount = 1;
		            streak.WeeksStreak = 0;

                    streak.UpdatedAtLocal = localDate;
                    Db.Update(streak);
	            }
            }

            private async Task<QuizResultQuestionEntity> GetOrCreateQuestion(Guid questionId, QuizResultEntity result, CancellationToken cancellationToken)
            {
                var question = result.Questions.FirstOrDefault(_ => _.QuestionId == questionId);

                if (question != null) 
                    return question;

                question = new QuizResultQuestionEntity
                           {
                               QuestionId = questionId,
                               QuizResultId = result.QuizStudentAssignmentId,
                           };
                await Db.AddAsync(question, cancellationToken);
                return question;
            }

            private async Task<QuizResultEntity> GetOrCreateQuizResult(QuizStudentAssignmentEntity quizStudentAssignmentEntity, CancellationToken cancellationToken)
            {
                if (quizStudentAssignmentEntity.Result != null) 
                    return quizStudentAssignmentEntity.Result;

                var quizResultEntity = new QuizResultEntity
                                       {
                                           QuizStudentAssignmentId = quizStudentAssignmentEntity.QuizStudentAssignmentId,
                                           Questions = new List<QuizResultQuestionEntity>(),
                                           StaredAt = DateTime.UtcNow,
                                           Score = 0
                                       };
                await Db.AddAsync(quizResultEntity, cancellationToken);
                return quizResultEntity;
            }
        }
    }
}

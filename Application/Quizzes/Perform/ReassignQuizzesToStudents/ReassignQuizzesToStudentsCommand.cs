using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Content.Services;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Quiz;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Quizzes.Perform.ReassignQuizzesToStudents
{
    [PublicAPI]
    public sealed class ReassignQuizzesToStudentsCommand : ICommand<ReassignQuizzesToStudentsCommand.Result>
    {
        public ImmutableList<Guid> QuizAssignmentIds { get; init; }
        public ImmutableList<Guid> StudentIds { get; init; }
        public DateTime DueDateUtc { get; init; }
        public DateTime DueDateLocal { get; init; }
        public DateTime AssignedDateUtc { get; init; }
        public DateTime AssignedDateLocal { get; init; }


        [PublicAPI]
        public record Result
        {
            public ImmutableList<Guid> QuizStudentAssignmentIds { get; init; }
        }


        internal sealed class Validator : ValidatorBase<ReassignQuizzesToStudentsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.QuizAssignmentIds).ExistsInTable(db.QuizAssignments);
                RuleFor(_ => _.QuizAssignmentIds).Must(BeAssignedTheSameClass).WithMessage("All quiz assignments must be related to the same class.");

                RuleForEach(_ => _.StudentIds).ExistsInTable(db.Students);
                RuleFor(_ => _.StudentIds)
                   .Must(BeRelatedToQuizAssignments)
                   .WithMessage("Some specified students don't related to the specified quiz assignments");
            }
            
            private bool BeAssignedTheSameClass(ImmutableList<Guid> quizAssignmentIds)
            {
                var relatedClassesCnt = Db.QuizAssignments
                                          .AsNoTracking()
                                          .Where(QuizAssignmentSpec.ByIds(quizAssignmentIds))
                                          .Select(_ => _.SchoolClassId)
                                          .Distinct()
                                          .Count();

                return relatedClassesCnt == 1;
            }

            private bool BeRelatedToQuizAssignments(ReassignQuizzesToStudentsCommand command, ImmutableList<Guid> studentIds)
            {
                var quizAssignmentClassId = Db.QuizAssignments
                                              .AsNoTracking()
                                              .Where(QuizAssignmentSpec.ByIds(command.QuizAssignmentIds))
                                              .Select(_ => _.SchoolClassId)
                                              .FirstOrDefault();

                return Db.Students
                         .Include(_ => _.SchoolClasses)
                         .AsNoTracking()
                         .Where(StudentSpec.ByIds(studentIds))
                         .All(_ => _.SchoolClasses.Any(c => c.SchoolClassId == quizAssignmentClassId));
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<ReassignQuizzesToStudentsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(ReassignQuizzesToStudentsCommand request, IUserSecurityInfo user)
            {
                var schoolId = _db.SchoolClasses
                                  .Where(SchoolClassSpec.WithQuizAssignments(request.QuizAssignmentIds))
                                  .Select(_ => _.SchoolId)
                                  .GetSingle();

                return user.IsSchoolTeacher(schoolId);
            }
        }

        internal sealed class Handler : CommandHandlerBase<ReassignQuizzesToStudentsCommand, Result>
        {
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;
            private readonly ISessionContext _sessionContext;
            private readonly ILearningMaterialNameService _learningMaterialNameService;

            public Handler(IDb db,
                           INotificationService notificationService,
                           ISiteUrls siteUrls,
                           ISessionContext sessionContext,
                           ILearningMaterialNameService learningMaterialNameService) : base(db)
            {
                _notificationService = notificationService;
                _siteUrls = siteUrls;
                _sessionContext = sessionContext;
                _learningMaterialNameService = learningMaterialNameService;
            }

            public override async Task<Result> Handle(ReassignQuizzesToStudentsCommand request, CancellationToken cancellationToken)
            {
                var existing = await Db.QuizStudentAssignments
                                       .AsNoTracking()
                                       .Where(QuizStudentAssignmentSpec.ForUsers(request.StudentIds))
                                       .Where(QuizStudentAssignmentSpec.ForQuizAssignments(request.QuizAssignmentIds))
                                       .Select(_ => new {_.QuizAssignmentId, _.AssignedToUserId, _.AttemptNumber})
                                       .ToListAsync(cancellationToken);

                var attemptMap = existing.GroupBy(_ => new { _.QuizAssignmentId, _.AssignedToUserId })
                                         .ToDictionary(_ => (_.Key.AssignedToUserId, _.Key.QuizAssignmentId),
                                                       _ => _.Select(x => x.AttemptNumber)
                                                             .Max());

                var reassigned = (from studentId in request.StudentIds
                                  from quizAssignmentId in request.QuizAssignmentIds
                                  let attemptNumber = attemptMap[(studentId, quizAssignmentId)] + 1
                                  select new QuizStudentAssignmentEntity
                                         {
                                             QuizStudentAssignmentId = Guid.NewGuid(),
                                             QuizAssignmentId = quizAssignmentId,
                                             AssignedToUserId = studentId,
                                             AttemptNumber = attemptNumber,
                                             DueAtLocal = request.DueDateLocal,
                                             DueAtUtc = request.DueDateUtc,
                                             AssignedAtLocal = request.AssignedDateLocal,
                                             AssignedAtUtc = request.AssignedDateUtc
                                         }).ToList();

                await Db.QuizStudentAssignments.AddRangeAsync(reassigned, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                await SendEmails(reassigned, _sessionContext.GetUserId(), request.DueDateLocal, cancellationToken);

                return new Result
                       {
                           QuizStudentAssignmentIds = reassigned.Select(_ => _.QuizStudentAssignmentId).ToImmutableList()
                       };
            }

            private async Task SendEmails(List<QuizStudentAssignmentEntity> assignments, Guid teacherId, DateTime dueDate, CancellationToken cancellationToken)
            {
                var quizStudentAssignments = await Db.QuizStudentAssignments
                                                     .Include(_ => _.QuizAssignment).ThenInclude(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                                     .Include(_ => _.QuizAssignment).ThenInclude(_ => _.SchoolClass).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                                                     .Include(_ => _.AssignedTo).ThenInclude(_ => _.Teacher)
                                                     .Include(_ => _.AssignedTo).ThenInclude(_ => _.Student)
                                                     .Where(QuizStudentAssignmentSpec.ByIds(assignments.Select(a => a.QuizStudentAssignmentId)))
                                                     .ToListAsync(cancellationToken);

                var schoolClass = quizStudentAssignments.Select(_ => _.QuizAssignment.SchoolClass).FirstOrDefault();
                var learningMaterialIds = quizStudentAssignments.Select(_ => _.QuizAssignment.Quiz.ContentTreeId).ToList();
                var materialsById = await _learningMaterialNameService.GetMaterialsById(learningMaterialIds, cancellationToken);

                var teacher = await Db.Teachers.GetSingleAsync(TeacherSpec.ById(teacherId), cancellationToken);

                foreach (var studentAssignments in quizStudentAssignments.GroupBy(_ => _.AssignedTo))
                {
                    var performQuizUrls = studentAssignments
                                          .Select(_ => new NotificationLink(materialsById[_.QuizAssignment.Quiz.ContentTreeId].fullName,
                                                                            _siteUrls.PerformQuiz(_.QuizStudentAssignmentId, _.QuizAssignment.Quiz.ContentTreeNode.CourseId)))
                                          .ToList();

                    await _notificationService.Send(new QuizzesAssignedNotification(studentAssignments.Key.Email, 
                                                                                    performQuizUrls, 
                                                                                    studentAssignments.Key.GetFullName(),
                                                                                    teacher.GetFullName(),
                                                                                    schoolClass?.Name ?? "",
                                                                                    dueDate,
                                                                                    schoolClass?.Course.Subject.Symbol ?? ""));
                }
            }
        }
    }
}

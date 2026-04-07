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
using Biobrain.Domain.Entities.Student;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Quizzes.Perform.AssignQuizzesToClass
{
    [PublicAPI]
    public sealed class AssignQuizzesToClassCommand : ICommand<AssignQuizzesToClassCommand.Result>
    {
        public ImmutableDictionary<Guid, ImmutableList<Guid>> StudentIdsBySchoolClassIdMap { get; init; }
        public ImmutableList<Guid> QuizIds { get; init; }
        public DateTime DueDateUtc { get; init; }
        public DateTime DueDateLocal { get; init; }
        public DateTime AssignedDateUtc { get; init; }
        public DateTime AssignedDateLocal { get; init; }
        public bool ForceCreateNew { get; init; } = false;
        public bool HintsEnabled { get; init; } = true;
        public bool SoundEnabled { get; init; } = true;
        public bool IncludeLearningMaterial { get; init; } = true;

        [PublicAPI]
        public record Result
        {
            public ImmutableList<Guid> QuizAssignmentIds { get; init; }
            public ImmutableList<Guid> NotAssignedQuizIds { get; init; }
        }


        internal sealed class Validator : ValidatorBase<AssignQuizzesToClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.StudentIdsBySchoolClassIdMap.Keys).ExistsInTable(Db.SchoolClasses).OverridePropertyName("SchoolClasses");
                RuleForEach(_ => _.StudentIdsBySchoolClassIdMap.Values.SelectMany(x => x)).ExistsInTable(db.Students).OverridePropertyName("Students");
                RuleFor(_ => _.QuizIds).NotNull();
                RuleFor(_ => _.QuizIds).NotEmpty();
                RuleFor(_ => _.QuizIds).Must(BeAvailableForSchool).WithMessage("Some quizzes are not available for the specified class.");
            }

            private bool BeAvailableForSchool(AssignQuizzesToClassCommand command, ImmutableList<Guid> quizIds)
            {
                var availableQuizIds = Db.Quizzes
                                         .Where(QuizSpec.ByIds(command.QuizIds))
                                          //.Where(QuizSpec.AvailableForClass(command.SchoolClassId)) // TODO: there is no relation yet
                                         .Select(_ => _.QuizId)
                                         .ToHashSet();

                return quizIds.All(availableQuizIds.Contains);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<AssignQuizzesToClassCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(AssignQuizzesToClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                List<Guid> studentIds = [..request.StudentIdsBySchoolClassIdMap.Values.SelectMany(x => x)];
                List<StudentEntity> students = _db.Students
                                                  .Include(x => x.Schools)
                                                  .Where(StudentSpec.ByIds(studentIds))
                                                  .ToList();

                return students.All(s => s.Schools.Any(_ => user.IsSchoolTeacher(_.SchoolId)));
            }
        }


        internal sealed class Handler : CommandHandlerBase<AssignQuizzesToClassCommand, Result>
        {
            private readonly ISessionContext _sessionContext;
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;
            private readonly ILearningMaterialNameService _learningMaterialNameService;

            public Handler(IDb db, ISessionContext sessionContext, INotificationService notificationService, ISiteUrls siteUrls, ILearningMaterialNameService learningMaterialNameService) : base(db)
            {
                _sessionContext = sessionContext;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
                _learningMaterialNameService = learningMaterialNameService;
            }

            public override async Task<Result> Handle(AssignQuizzesToClassCommand request, CancellationToken cancellationToken)
            {
                var studentIds = await Db.Students
                                         .Where(StudentSpec.ByIds(request.StudentIdsBySchoolClassIdMap.Values.SelectMany(x => x)))
                                         .Select(_ => _.StudentId)
                                         .ToListAsync(cancellationToken);

                ImmutableHashSet<Guid> studentIdSet = studentIds.ToImmutableHashSet();

                // Load class-level setting overrides so teachers can force hints/sound off for a whole class.
                var classIdList = request.StudentIdsBySchoolClassIdMap.Keys.ToList();
                var classHintsDisabledSet = (await Db.SchoolClasses
                                                     .Where(c => classIdList.Contains(c.SchoolClassId) && c.HintsDisabled)
                                                     .Select(c => c.SchoolClassId)
                                                     .ToListAsync(cancellationToken)).ToHashSet();
                var classSoundDisabledSet = (await Db.SchoolClasses
                                                     .Where(c => classIdList.Contains(c.SchoolClassId) && c.SoundDisabled)
                                                     .Select(c => c.SchoolClassId)
                                                     .ToListAsync(cancellationToken)).ToHashSet();

                var assignments = (from quizId in request.QuizIds
                                   from classId in request.StudentIdsBySchoolClassIdMap.Keys
                                   let quizAssignmentId = Guid.NewGuid()
                                   let classHintsDisabled = classHintsDisabledSet.Contains(classId)
                                   let classSoundDisabled = classSoundDisabledSet.Contains(classId)
                                   let studentAssignments = from studentId in request.StudentIdsBySchoolClassIdMap[classId]
                                                            where studentIdSet.Contains(studentId)
                                                            select new QuizStudentAssignmentEntity
                                                                   {
                                                                       QuizStudentAssignmentId = Guid.NewGuid(),
                                                                       QuizAssignmentId = quizAssignmentId,
                                                                       AssignedToUserId = studentId,
                                                                       AttemptNumber = 1,
                                                                       DueAtLocal = request.DueDateLocal,
                                                                       DueAtUtc = request.DueDateUtc,
                                                                       AssignedAtLocal = request.AssignedDateLocal,
                                                                       AssignedAtUtc = request.AssignedDateUtc
                                                                   }
                                   select new QuizAssignmentEntity
                                          {
                                              QuizAssignmentId = quizAssignmentId,
                                              QuizId = quizId,
                                              SchoolClassId = classId,
                                              AssignedByTeacherId = _sessionContext.GetUserId(),
                                              QuizStudentAssignments = studentAssignments.ToList(),
                                              DueAtLocal = request.DueDateLocal,
                                              DueAtUtc = request.DueDateUtc,
                                              AssignedAtLocal = request.AssignedDateLocal,
                                              AssignedAtUtc = request.AssignedDateUtc,
                                              HintsEnabled = request.HintsEnabled && !classHintsDisabled,
                                              SoundEnabled = request.SoundEnabled && !classSoundDisabled,
                                              IncludeLearningMaterial = request.IncludeLearningMaterial
                                          }).ToList();

                var notAssignedQuizIds = new List<Guid>();
                if (!request.ForceCreateNew)
                    notAssignedQuizIds.AddRange(await MergeWithExistingAssignments(assignments,
                                                                                   _sessionContext.GetUserId(),
                                                                                   request.StudentIdsBySchoolClassIdMap.Keys,
                                                                                   cancellationToken));

                await Db.QuizAssignments.AddRangeAsync(assignments, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                foreach (Guid schoolClassId in request.StudentIdsBySchoolClassIdMap.Keys)
                    await SendEmails(assignments, _sessionContext.GetUserId(), schoolClassId, request.DueDateLocal, cancellationToken);

                return new Result
                       {
                           QuizAssignmentIds = assignments.Select(_ => _.QuizAssignmentId).ToImmutableList(),
                           NotAssignedQuizIds = notAssignedQuizIds.ToImmutableList()
                       };
            }

            private async Task<IEnumerable<Guid>> MergeWithExistingAssignments(List<QuizAssignmentEntity> assignments,
                                                                               Guid teacherId,
                                                                               IEnumerable<Guid> classIds,
                                                                               CancellationToken ct)
            {
                var existingAssignments = await Db.QuizAssignments
                    .Include(x => x.QuizStudentAssignments)
                    .Where(QuizAssignmentSpec.ByClassIds(classIds))
                    .Where(QuizAssignmentSpec.AssignedByTeacher(teacherId))
                    .Where(QuizAssignmentSpec.ByQuizIds(assignments.Select(_ => _.QuizId)))
                    .Where(QuizAssignmentSpec.FromDate(DateTime.UtcNow.AddHours(-48)))
                    .ToListAsync(ct);

                var toNoCreate = new List<QuizAssignmentEntity>();
                var alreadyExist = new HashSet<Guid>();

                foreach (var newAssignment in assignments)
                {
                    var existingAssignment = existingAssignments.FirstOrDefault(_ => _.QuizId == newAssignment.QuizId);
                    if (existingAssignment == null)
                        continue;

                    // All new students exist in old assignment -> need to ask user: create new or cancel
                    toNoCreate.Add(newAssignment);
                    if (newAssignment.QuizStudentAssignments.All(n =>
                            existingAssignment.QuizStudentAssignments.Any(o => n.AssignedToUserId == o.AssignedToUserId)))
                    {
                        //throw new ValidationException("One or more assignments was already created for this students in 48 hours");
                        alreadyExist.Add(existingAssignment.QuizId);
                        continue;
                    }

                    // New students contains not exist in old assignment -> add to the old assignment and update due date and remove from new list
                    var newStudentAssignments = newAssignment.QuizStudentAssignments.Where(_ =>
                        existingAssignment.QuizStudentAssignments.All(s => s.AssignedToUserId != _.AssignedToUserId));
                    foreach (var newStudentAssignment in newStudentAssignments)
                    {
                        newStudentAssignment.QuizAssignmentId = existingAssignment.QuizAssignmentId;
                        Db.QuizStudentAssignments.Add(newStudentAssignment);
                    }

                    // Update due dates
                    existingAssignment.DueAtLocal = newAssignment.DueAtLocal;
                    existingAssignment.DueAtUtc = newAssignment.DueAtUtc;
                    foreach (var existingUserAssignment in existingAssignment.QuizStudentAssignments)
                    {
                        existingUserAssignment.DueAtLocal = newAssignment.DueAtLocal;
                        existingUserAssignment.DueAtUtc = newAssignment.DueAtUtc;
                    }
                }

                assignments.RemoveAll(_ => toNoCreate.Contains(_));
                return alreadyExist;
            }

            private async Task SendEmails(List<QuizAssignmentEntity> assignments, Guid teacherId, Guid schoolClassId, DateTime dueDate, CancellationToken cancellationToken)
            {
                var quizStudentAssignments = await Db.QuizStudentAssignments
                                                     .Include(_ => _.QuizAssignment).ThenInclude(_ => _.Quiz).ThenInclude(_ => _.ContentTreeNode)
                                                     .Include(_ => _.AssignedTo).ThenInclude(_ => _.Teacher)
                                                     .Include(_ => _.AssignedTo).ThenInclude(_ => _.Student)
                                                     .Where(QuizStudentAssignmentSpec.ByIds(assignments.SelectMany(_ => _.QuizStudentAssignments).Select(a => a.QuizStudentAssignmentId)))
                                                     .ToListAsync(cancellationToken);
                var schoolClass = await Db.SchoolClasses.Where(SchoolClassSpec.ById(schoolClassId))
                    .Include(_ => _.Course)
                    .ThenInclude(_ => _.Subject)
                    .FirstOrDefaultAsync(cancellationToken);

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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Content.Services;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Specifications;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.LearningMaterialAssignments.Services
{
    internal sealed class AssignLearningMaterialsNotificationService : IAssignLearningMaterialsNotificationService
    {
        private readonly IDb _db;
        private readonly INotificationService _notificationService;
        private readonly ISiteUrls _siteUrls;
        private readonly ISessionContext _sessionContext;
        private readonly ILearningMaterialNameService _learningMaterialNameService;

        public AssignLearningMaterialsNotificationService(IDb db,
                                                          INotificationService notificationService,
                                                          ISiteUrls siteUrls,
                                                          ISessionContext sessionContext,
                                                          ILearningMaterialNameService learningMaterialNameService)
        {
            _db = db;
            _notificationService = notificationService;
            _siteUrls = siteUrls;
            _sessionContext = sessionContext;
            _learningMaterialNameService = learningMaterialNameService;
        }

        public async Task Send(IAssignLearningMaterialService.Result assignResult, Guid schoolClassId, DateTime dueDateLocal, CancellationToken ct)
        {
            var assignmentsByUserMap = assignResult.Rows.ToLookup(_ => _.UserId);

            var schoolClass = await _db.SchoolClasses.Where(SchoolClassSpec.ById(schoolClassId))
                .Include(_ => _.Course)
                .ThenInclude(_ => _.Subject)
                .FirstOrDefaultAsync(ct);

            var students = await _db.Students
                                    .Include(_ => _.User)
                                    .Where(StudentSpec.ByIds(assignmentsByUserMap.Select(_ => _.Key)))
                                    .ToListAsync(ct);

            var contentTreeNodeIds = assignResult.Rows.Select(_ => _.ContentTreeNodeId).Distinct();

            var materialsById = await _learningMaterialNameService.GetMaterialsById(contentTreeNodeIds, ct);

            var teacher = await _db.Teachers.GetSingleAsync(TeacherSpec.ById(_sessionContext.GetUserId()), ct);

            foreach (var student in students)
            {
                var links = from assignment in assignmentsByUserMap[student.StudentId]
                            let node = materialsById[assignment.ContentTreeNodeId]
                            select new NotificationLink(node.fullName, _siteUrls.PerformAssignedLearningMaterial(assignment.LearningMaterialUserAssignmentId));

                await _notificationService.Send(new LearningMaterialsAssignedNotification(student.User.Email,
                                                                                          links.ToList(),
                                                                                          student.GetFullName(),
                                                                                          teacher.GetFullName(),
                                                                                          schoolClass?.Name ?? "",
                                                                                          dueDateLocal,
                                                                                          schoolClass?.Course.Subject.Symbol ?? ""));
            }
        }
    }
}

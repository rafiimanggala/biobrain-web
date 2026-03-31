using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.MaterialAssignments;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.LearningMaterialAssignments.Services
{
    internal sealed class AssignLearningMaterialService : IAssignLearningMaterialService
    {
        private readonly IDb _db;
        private readonly ISessionContext _sessionContext;

        public AssignLearningMaterialService(IDb db, ISessionContext sessionContext)
        {
            _db = db;
            _sessionContext = sessionContext;
        }

        public async Task<IAssignLearningMaterialService.Result> Assign(IAssignLearningMaterialService.Params request, bool forceCreateNew, CancellationToken ct)
        {
            var studentIds = request.StudentIds ??
                             await _db.Students
                                      .Include(_ => _.User)
                                      .Where(StudentSpec.ForClass(request.SchoolClassId))
                                      .Select(_ => _.StudentId)
                                      .ToImmutableListAsync(ct);

            var assignments = (from nodeId in request.ContentTreeNodeIds
                               let assignmentId = Guid.NewGuid()
                               let studentAssignments = from studentId in studentIds
                                                        select new LearningMaterialUserAssignmentEntity
                                                               {
                                                                   LearningMaterialUserAssignmentId = Guid.NewGuid(),
                                                                   LearningMaterialAssignmentId = assignmentId,
                                                                   AssignedToUserId = studentId,
                                                                   DueAtLocal = request.DueDateLocal,
                                                                   DueAtUtc = request.DueDateUtc,
                                                                   AssignedAtLocal = request.AssignedDateLocal,
                                                                   AssignedAtUtc = request.AssignedDateUtc
                                                               }
                               select new LearningMaterialAssignmentEntity
                                      {
                                          LearningMaterialAssignmentId = assignmentId,
                                          ContentTreeNodeId = nodeId,
                                          SchoolClassId = request.SchoolClassId,
                                          AssignedByUserId = _sessionContext.GetUserId(),
                                          UserAssignments = studentAssignments.ToList(),
                                          DueAtLocal = request.DueDateLocal,
                                          DueAtUtc = request.DueDateUtc,
                                          AssignedAtLocal = request.AssignedDateLocal,
                                          AssignedAtUtc = request.AssignedDateUtc,
                                          IsForWholeClass = request.StudentIds is null
                                      }).ToList();

            var notAssignedNodeIds = new List<Guid>();
            if(!forceCreateNew)
                notAssignedNodeIds.AddRange(await MergeWithExistingAssignments(assignments, _sessionContext.GetUserId(), request.SchoolClassId, ct));

            await _db.LearningMaterialAssignments.AddRangeAsync(assignments, ct);
            await _db.SaveChangesAsync(ct);

            var resultRows = from assignment in assignments
                             let nodeId = assignment.ContentTreeNodeId
                             from userAssignment in assignment.UserAssignments
                             select new IAssignLearningMaterialService.Result.Row
                                    {
                                        LearningMaterialUserAssignmentId = userAssignment.LearningMaterialUserAssignmentId,
                                        UserId = userAssignment.AssignedToUserId,
                                        ContentTreeNodeId = nodeId
                                    };

            return new IAssignLearningMaterialService.Result
                   {
                       Rows = resultRows.ToImmutableList(),
                       NotAssignedNodeIds = notAssignedNodeIds.ToImmutableList()
            };
        }

        private async Task<IEnumerable<Guid>> MergeWithExistingAssignments(List<LearningMaterialAssignmentEntity> assignments, Guid teacherId, Guid classId, CancellationToken ct)
        {
            var existingAssignments = await _db.LearningMaterialAssignments
                .Include(x => x.UserAssignments)
                .Where(LearningMaterialAssignmentSpec.ByClassId(classId))
                .Where(LearningMaterialAssignmentSpec.AssignedByTeacher(teacherId))
                .Where(LearningMaterialAssignmentSpec.ByContentTreeNodeIds(assignments.Select(_ => _.ContentTreeNodeId)))
                .Where(LearningMaterialAssignmentSpec.FromDate(DateTime.UtcNow.AddHours(-48)))
                .ToListAsync(ct);

            var toNoCreate = new List<LearningMaterialAssignmentEntity>();
            var notAssignedNodeIds = new List<Guid>();
            foreach (var newAssignment in assignments)
            {
                var existingAssignment =
                    existingAssignments.FirstOrDefault(_ => _.ContentTreeNodeId == newAssignment.ContentTreeNodeId);
                if(existingAssignment == null) continue;

                toNoCreate.Add(newAssignment);
                // All new students exist in old assignment -> need to ask user: create new or cancel
                if (newAssignment.UserAssignments.All(n =>
                        existingAssignment.UserAssignments.Any(o => n.AssignedToUserId == o.AssignedToUserId)))
                {
                    notAssignedNodeIds.Add(existingAssignment.ContentTreeNodeId);
                    continue;
                }

                // New students contains not exist in old assignment -> add to the old assignment and update due date and remove from new list
                var newStudentAssignments = newAssignment.UserAssignments.Where(_ =>
                    existingAssignment.UserAssignments.All(s => s.AssignedToUserId != _.AssignedToUserId));
                foreach (var newStudentAssignment in newStudentAssignments)
                {
                    newStudentAssignment.LearningMaterialAssignmentId = existingAssignment.LearningMaterialAssignmentId;
                    _db.LearningMaterialUserAssignments.Add(newStudentAssignment);
                }

                // Update due dates
                existingAssignment.DueAtLocal = newAssignment.DueAtLocal;
                existingAssignment.DueAtUtc = newAssignment.DueAtUtc;
                foreach (var existingUserAssignment in existingAssignment.UserAssignments)
                {
                    existingUserAssignment.DueAtLocal = newAssignment.DueAtLocal;
                    existingUserAssignment.DueAtUtc = newAssignment.DueAtUtc;
                }
            }

            assignments.RemoveAll(_ => toNoCreate.Contains(_));
            return notAssignedNodeIds;
        }
    }
}

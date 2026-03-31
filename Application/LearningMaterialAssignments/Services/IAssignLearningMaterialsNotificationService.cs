using System;
using System.Threading;
using System.Threading.Tasks;


namespace Biobrain.Application.LearningMaterialAssignments.Services
{
    internal interface IAssignLearningMaterialsNotificationService
    {
        Task Send(IAssignLearningMaterialService.Result assignResult, Guid schoolClassId, DateTime dueDateLocal, CancellationToken ct);
    }
}

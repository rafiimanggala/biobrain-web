using System;
using System.Threading.Tasks;

namespace Biobrain.Application.Services.AI
{
    public interface IPerformanceInsightsService
    {
        Task<string> GenerateInsightsForClassAsync(Guid schoolClassId, Guid courseId, DateTime fromDate, DateTime toDate);
        Task SendWeeklyInsightsAsync();
    }
}

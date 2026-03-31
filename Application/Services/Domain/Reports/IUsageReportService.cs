using System.Threading.Tasks;
using System.Threading;
using System;

namespace Biobrain.Application.Services.Domain.Reports
{
    public interface IUsageReportService
    {
        Task<string> GetSchoolReport(Guid schoolId, string timeZoneId, DateTime from, DateTime to,
            string tempReportFolder, CancellationToken cancellationToken);
    }
}
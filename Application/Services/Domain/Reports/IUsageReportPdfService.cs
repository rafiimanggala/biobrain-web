using Biobrain.Application.Common.Models;

namespace Biobrain.Application.Services.Domain.Reports
{
    public interface IUsageReportPdfService
    {
        void GeneratePdf(UsageReportModel model);
    }
}
using System;
using System.Collections.Generic;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Reports.UsageReport;
using Biobrain.Domain.Constants;

namespace Biobrain.Application.Services.Domain.Reports
{
    public interface IUsageReportChartService
    {
        ChartModel GetPagesPieChartImagePath(string folderPath, Dictionary<string, int> pageViewsDictionary);

        ChartModel GetLinearChartImagePath(string folderPath,
            Dictionary<ReportKeyData, List<int>> subjectNumberOfSessionsDictionary, DateTime startDate, Constant.TimeInterval interval);
    }
}
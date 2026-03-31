using System.Collections.Generic;
using Biobrain.Application.Reports.UsageReport;

namespace Biobrain.Application.Common.Models
{
    public class UsageReportModel
    {
        public string ReportPath { get; set; }
        public string SchoolName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int TotalNumberOfSessions { get; set; }
        public double AverageSessionDuration { get; set; }

        public Dictionary<ReportKeyData, List<int>> NumberOfSessionsByPeriod { get; set; } 
        public ChartModel SessionsChart { get; set; }


        public Dictionary<ReportKeyData, List<int>> NumberOfTeacherUsersByPeriod { get; set; } 
        public ChartModel TeacherUsersChart { get; set; }
        public Dictionary<ReportKeyData, List<int>> NumberOfStudentUsersByPeriod { get; set; }
        public ChartModel StudentUsersChart { get; set; }

        public ChartModel PagesChart { get; set; }
    }
}
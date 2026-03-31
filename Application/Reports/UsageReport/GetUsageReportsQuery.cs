using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.Reports;
using BiobrainWebAPI.Values;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace Biobrain.Application.Reports.UsageReport
{
    [PublicAPI]
    public sealed class GetUsageReportsQuery : ICommand<GetUsageReportsQuery.Result>
    {
        public List<Guid> Schools { get; set; }
        public DateTime FromDateUtc { get; init; }
        public DateTime ToDateUtc { get; init; }
        public string TimeZoneId { get; set; }

        [PublicAPI]
        public record Result
        {
	        public string FileUrl { get; set; }
        }

        internal class Validator : ValidatorBase<GetUsageReportsQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.Schools).ExistsInTable(Db.Schools);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetUsageReportsQuery>
        {

            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetUsageReportsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<GetUsageReportsQuery, Result>
        {
            private readonly IConfiguration _configuration;
            private readonly IUsageReportService _usageReport;

            public Handler(IDb db, IConfiguration configuration, IUsageReportService usageReport) : base(db)
            {
                _configuration = configuration;
                _usageReport = usageReport;
            }

            public override async Task<Result> Handle(GetUsageReportsQuery request, CancellationToken cancellationToken)
            {
                var fileName = Guid.NewGuid() + ".pdf";
                var reportsFolder = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink);
                var tmpReportFolder = Path.Combine(reportsFolder, Guid.NewGuid().ToString());
                if(!Directory.Exists(tmpReportFolder)) Directory.CreateDirectory(tmpReportFolder);

                var fileNames = new List<string>();
                foreach (var schoolId in request.Schools)
                {
                    var reportFileName = await _usageReport.GetSchoolReport(schoolId, request.TimeZoneId, request.FromDateUtc, request.ToDateUtc, tmpReportFolder, cancellationToken);
                    fileNames.Add(reportFileName);
                }

                CleanFolderFromTmpFiles(tmpReportFolder);
                //ZipFile.CreateFromDirectory(tmpReportFolder, Path.Combine(reportsFolder, fileName));
                MergePDFs(Path.Combine(reportsFolder, fileName), Directory.EnumerateFiles(tmpReportFolder).ToList());

                foreach (var report in Directory.EnumerateFiles(tmpReportFolder))
                {
                    if(File.Exists(report))
                        File.Delete(report);
                }
                
                if(Directory.Exists(tmpReportFolder))
                    Directory.Delete(tmpReportFolder);

                return new Result
	                {FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString()};
            }

            private void CleanFolderFromTmpFiles(string folderPath)
            {
                foreach (var file in Directory.EnumerateFiles(folderPath))
                {
                    var fileInfo = new FileInfo(file);
                    if (File.Exists(file) && fileInfo.Extension != ".pdf")
                        File.Delete(file);
                }
            }

            public static void MergePDFs(string targetPath, List<string> pdfs)
            {
                using (var targetDoc = new PdfDocument())
                {
                    foreach (var pdf in pdfs)
                    {
                        using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                        {
                            for (var i = 0; i < pdfDoc.PageCount; i++)
                                targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                    targetDoc.Save(targetPath);
                }
            }

        }
    }
}

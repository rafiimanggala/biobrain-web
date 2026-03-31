using System;
using System.IO;
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

namespace Biobrain.Application.Reports.UsageReport
{
    [PublicAPI]
    public sealed class GetUsageReportQuery : ICommand<GetUsageReportQuery.Result>
    {
        public Guid SchoolId { get; set; }
        public DateTime FromDateUtc { get; init; }
        public DateTime ToDateUtc { get; init; }
        public string TimeZoneId { get; set; }

        [PublicAPI]
        public record Result
        {
	        public string FileUrl { get; set; }
        }

        internal class Validator : ValidatorBase<GetUsageReportQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetUsageReportQuery>
        {

            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetUsageReportQuery request, IUserSecurityInfo user) => user.IsSchoolAdmin(request.SchoolId) || user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<GetUsageReportQuery, Result>
        {
            private readonly IConfiguration _configuration;
            private readonly IUsageReportService _usageReport;

            public Handler(IDb db, IConfiguration configuration, IUsageReportService usageReport) : base(db)
            {
                _configuration = configuration;
                _usageReport = usageReport;
            }

            public override async Task<Result> Handle(GetUsageReportQuery request, CancellationToken cancellationToken)
            {
                var reportsFolder = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink);
                var reportFileName = await _usageReport.GetSchoolReport(request.SchoolId, request.TimeZoneId, request.FromDateUtc, request.ToDateUtc, reportsFolder, cancellationToken);

                return new Result
	                {FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{reportFileName}", UriKind.Relative).ToString()};
            }

        }
    }
}

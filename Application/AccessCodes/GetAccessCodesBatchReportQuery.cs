using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Helpers;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AccessCode;
using Biobrain.Domain.Entities.AccessCodes;
using BiobrainWebAPI.Values;
using Csv;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.AccessCodes;

[PublicAPI]
public sealed class GetAccessCodesBatchReportQuery : IQuery<GetAccessCodesBatchReportQuery.Result>
{
    public Guid BatchId { get; set; }
    public string TimeZoneId { get; set; }


    [PublicAPI]
    public class Result
    {
        public string FileUrl { get; set; }
    }

    internal sealed class Validator : ValidatorBase<GetAccessCodesBatchReportQuery>
    {
        public Validator(IDb db) : base(db)
        {
            RuleFor(_ => _.BatchId).ExistsInTable(Db.AccessCodeBatches);
        }
    }

    internal sealed class PermissionCheck : PermissionCheckBase<GetAccessCodesBatchReportQuery>
    {
        public PermissionCheck(ISecurityService securityService) 
            : base(securityService)
        {
        }

        protected override bool CanExecute(GetAccessCodesBatchReportQuery request, IUserSecurityInfo user)
        {
            if (user.IsApplicationAdmin())
                return true;

            return false;
        }
    }


    internal sealed class Handler(IDb db, IConfiguration configuration, IAccessCodeService accessCodeService)
        : QueryHandlerBase<GetAccessCodesBatchReportQuery, Result>(db)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IAccessCodeService _accessCodeService = accessCodeService;

        public override async Task<Result> Handle(GetAccessCodesBatchReportQuery request, CancellationToken cancellationToken)
        {
            string[] header = ["Access Code", "Date Created", "Date Redeemed", "Date Code Expires"];
            AccessCodeBatchEntity batch = await GetAccessCodeBatch(request, cancellationToken);
            List<string[]> rows = GetRows(request, batch);

            string fileName = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink, fileName);

            const string rfc4180Separator = "\r\n";

            string csv = $"{_accessCodeService.GetBatchHeader(batch)}" +
                         $"{rfc4180Separator}" +
                         $"{CsvWriter.WriteToText(header, rows)}";

            await File.WriteAllTextAsync(filePath, csv, cancellationToken);

            return new Result
                   { FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString() };
        }

        private Task<AccessCodeBatchEntity> GetAccessCodeBatch(GetAccessCodesBatchReportQuery request, CancellationToken ct)
            => Db.AccessCodeBatches.AsNoTracking()
                 .Include(x => x.UsedAccessCodes)
                 .Include(x => x.Courses)
                 .ThenInclude(x => x.Course)
                 .ThenInclude(x => x.Subject)
                 .Include(x => x.Courses)
                 .ThenInclude(x => x.Course)
                 .ThenInclude(x => x.Curriculum)
                 .Where(x => x.AccessCodeBatchId == request.BatchId)
                 .OrderByDescending(x => x.CreatedAt)
                 .ThenBy(x => x.NumberOfCodes)
                 .SingleAsync(ct);

        private static List<string[]> GetRows(GetAccessCodesBatchReportQuery request, AccessCodeBatchEntity batch)
            => batch.UsedAccessCodes
                    .Select(x => new[]
                                 {
                                     x.Code,
                                     TimeZoneInfoHelper.ConvertTimeFromUtc(batch.CreatedAt, request.TimeZoneId).ToString("dd-MM-yyyy HH:mm"),
                                     TimeZoneInfoHelper.ConvertTimeFromUtc(x.CreatedAt, request.TimeZoneId).ToString("dd-MM-yyyy HH:mm"),
                                     batch.ExpiryDate == null
                                         ? "-"
                                         : TimeZoneInfoHelper.ConvertTimeFromUtc(batch.ExpiryDate.Value, request.TimeZoneId).ToString("dd-MM-yyyy HH:mm")
                                 })
                    .ToList();
    }
}
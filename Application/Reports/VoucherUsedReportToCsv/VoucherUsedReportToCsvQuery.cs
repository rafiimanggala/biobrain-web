using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Helpers;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using BiobrainWebAPI.Values;
using Csv;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.Reports.VoucherUsedReportToCsv
{
    [PublicAPI]
    public sealed class VoucherUsedReportToCsvQuery : IQuery<VoucherUsedReportToCsvQuery.Result>
    {
        public DateTime FromDateUtc { get; init; }
        public DateTime ToDateUtc { get; init; }
        public string TimeZoneId { get; set; }

        [PublicAPI]
        public record Result
        {
	        public string FileUrl { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<VoucherUsedReportToCsvQuery>
        {

            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(VoucherUsedReportToCsvQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }

        internal sealed class Handler : QueryHandlerBase<VoucherUsedReportToCsvQuery, Result>
        {
            private readonly IConfiguration _configuration;

            public Handler(IDb db, IConfiguration configuration) : base(db) => _configuration = configuration;

            public override async Task<Result> Handle(VoucherUsedReportToCsvQuery request, CancellationToken cancellationToken)
            {

	            var header = new List<string> { "TransactionDate", "VoucherCreatedAt", "VoucherUsedAt", "SupplierVoucherCode", "VoucherAmount", "VoucherAmountUsed", "TransactionAmount", "Category", "School" }.ToArray();
                var rows = await GetRows(request, cancellationToken);

                var fileName = Guid.NewGuid() + ".csv";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink, fileName);



                var csv = CsvWriter.WriteToText(header, rows);
                await File.WriteAllTextAsync(filePath, csv, cancellationToken);

                return new Result
	                {FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString()};
            }

            private async Task<List<string[]>> GetRows(VoucherUsedReportToCsvQuery request, CancellationToken cancellationToken)
            {

	            var vouchers = await Db.UserVoucherTransactions.AsNoTracking()
		            .Include(_ => _.UserVoucher).ThenInclude(_ => _.Voucher)
                    .Where(_ => _.UpdatedAt >= request.FromDateUtc && _.UpdatedAt <= request.ToDateUtc)
		            .OrderByDescending(_ => _.CreatedAt)
		            .ToListAsync(cancellationToken);

                var rows = new List<string[]>();

                vouchers.ForEach(p =>
                {
                    var row = new List<string>
                    {
                        // TransactionDate
		                p.CreatedAt.ToString("O"),
                        // CreatedAt
                        TimeZoneInfoHelper.ConvertTimeFromUtc((p.UserVoucher?.Voucher?.CreatedAt ?? DateTime.MinValue), request.TimeZoneId)
                        .ToString("dd-MM-yyyy HH:mm") ?? "",
                        // VoucherUsedAt
                        TimeZoneInfoHelper.ConvertTimeFromUtc(p.CreatedAt, request.TimeZoneId)
                            .ToString("dd-MM-yyyy HH:mm") ?? "",
                        //SupplierVoucherCode
		                p.UserVoucher?.Voucher?.Code ?? "",
                        // Amount
		                p.UserVoucher?.Voucher?.TotalAmount.ToString(".##") ?? "",
                        //AmountUsed
                        p.UserVoucher?.Voucher?.AmountUsed.ToString(".##"),
                        //TransactionAmount
                        p.Amount.ToString(".##"),
                        // Category
		                "Textbook",
                        //School
                        p.UserVoucher?.SchoolName ?? ""
                        
                    };

                    rows.Add(row.ToArray());
                });

                return rows;
            }
            
        }
    }
}

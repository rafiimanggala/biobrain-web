using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Helpers;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using BiobrainWebAPI.Values;
using Csv;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.Reports.PurchasesReportToCsv
{
    [PublicAPI]
    public sealed class PurchaseReportToCsvQuery : IQuery<PurchaseReportToCsvQuery.Result>
    {
        public DateTime FromDateUtc { get; init; }
        public DateTime ToDateUtc { get; init; }
        public string TimeZoneId { get; set; }

        [PublicAPI]
        public record Result
        {
	        public string FileUrl { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<PurchaseReportToCsvQuery>
        {

            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(PurchaseReportToCsvQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }

        internal sealed class Handler : QueryHandlerBase<PurchaseReportToCsvQuery, Result>
        {
            private readonly IConfiguration _configuration;

            public Handler(IDb db, IConfiguration configuration) : base(db) => _configuration = configuration;

            public override async Task<Result> Handle(PurchaseReportToCsvQuery request, CancellationToken cancellationToken)
            {

	            var header = new List<string> { "Country", "State", "Curriculum", "Subjects", "Number Of Subjects", "Date", "Cost", "Payment Status", "", "Subjects in subscription", "Number Of Subjects in subscription", "Current subscription amount", "Subscription status" }.ToArray();
                var rows = await GetRows(request, cancellationToken);

                var fileName = Guid.NewGuid() + ".csv";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink, fileName);



                var csv = CsvWriter.WriteToText(header, rows);
                await File.WriteAllTextAsync(filePath, csv, cancellationToken);

                return new Result
	                {FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString()};
            }

            private async Task<List<string[]>> GetRows(PurchaseReportToCsvQuery request, CancellationToken cancellationToken)
            {

	            var payments = await Db.Payment.AsNoTracking()
		            .Include(_ => _.ScheduledPayment).ThenInclude(_ => _.User).ThenInclude(_ => _.Student).ThenInclude(_ => _.Curriculum)
                    //.Include(_ => _.ScheduledPayment).ThenInclude(_ => _.User).ThenInclude(_ => _.Teacher)
                    .Include(_ => _.ScheduledPayment).ThenInclude(_ => _.ScheduledPaymentCourses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
		            .Include(_ => _.ScheduledPayment).ThenInclude(_ => _.ScheduledPaymentCourses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Curriculum)
                    .Where(PaymentSpec.ForDates(request.FromDateUtc, request.ToDateUtc))
                    .Where(PaymentSpec.Succeeded())
		            .OrderByDescending(_ => _.CreatedAt)
		            .ToListAsync(cancellationToken);

                var rows = new List<string[]>();

                payments.ForEach(p =>
                {
	                var row = new List<string>
	                {
                        // Country
		                p.ScheduledPayment.User?.Student?.Country ?? "",
                        //State
		                p.ScheduledPayment.User?.Student?.State ?? "",
                        // Curriculum
		                p.ScheduledPayment.User?.Student?.Curriculum?.Name ?? "",
                        // Subjects
		                string.IsNullOrEmpty(p.ProductDescription)
			                ? " - "
			                : p.ProductDescription,
                        // Subjects count
		                string.IsNullOrEmpty(p.ProductDescription) ? " - " : p.ProductDescription.Split(',').Length.ToString(),
                        // Date
		                TimeZoneInfoHelper.ConvertTimeFromUtc(p.CreatedAt, request.TimeZoneId)
			                .ToString("dd-MM-yyyy HH:mm") ?? "",
                        // Amount
		                string.IsNullOrEmpty(p.Amount) ? " - " : p.Amount,
                        // PaymentStatus
                        p.Status.ToString(),

                        //Separator to subscription data
                        "",


                        // Current subscription subjects
						string.Join(", ",
						 p.ScheduledPayment.ScheduledPaymentCourses.Where(x => x.Status == ScheduledPaymentCourseStatus.Active).Select(c =>
						  $"{c.Course.Subject.Name}{(c.Course.Curriculum.IsGeneric ? "" : $" - Year {c.Course.Year}")}")),
                        // Current subscription subjects count
                        p.ScheduledPayment.ScheduledPaymentCourses.Count(x => x.Status == ScheduledPaymentCourseStatus.Active).ToString() ?? "",
                        //SubscriptionAmount
                        $"{p.ScheduledPayment.Currency}{p.ScheduledPayment.Amount}" ?? "",
                        // Subscription status
                        p.ScheduledPayment.Status.ToString()
					};

                    // 

                    rows.Add(row.ToArray());
                });

	            return rows;
            }
            
        }
    }
}

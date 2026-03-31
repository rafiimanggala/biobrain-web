using System.Threading.Tasks;
using Biobrain.Application.Reports.ContentStructure;
using Biobrain.Application.Reports.PurchasesReportToCsv;
using Biobrain.Application.Reports.UsageReport;
using Biobrain.Application.Reports.VoucherUsedReportToCsv;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
		public Task<ActionResult<PurchaseReportToCsvQuery.Result>> GetPurchaseReportToCsv([FromQuery] PurchaseReportToCsvQuery query)
			=> _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GetUsageReportsQuery.Result>> GetUsageReports(GetUsageReportsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GetUsageReportQuery.Result>> GetUsageReport(GetUsageReportQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetContentStructureToCsvQuery.Result>> GetContentStructureToCsv([FromQuery] GetContentStructureToCsvQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<VoucherUsedReportToCsvQuery.Result>> VoucherUsedReportToCsv([FromQuery] VoucherUsedReportToCsvQuery query)
            => _mediator.Send(query).ToActionResult();

        //[HttpPost]
        //public Task<ActionResult<UpdateSchoolClassCommand.Result>> UpdateSchoolClass(UpdateSchoolClassCommand command)
        // => _mediator.Send(command).ToActionResult();
    }
}
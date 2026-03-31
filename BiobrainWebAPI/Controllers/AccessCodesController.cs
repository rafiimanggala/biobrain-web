using System.Threading.Tasks;
using Biobrain.Application.AccessCodes;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccessCodesController : Controller
    {
        private readonly IMediator _mediator;

        public AccessCodesController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet]
        public Task<ActionResult<GetAccessCodesQuery.Result>> GetAccessCodes([FromQuery] GetAccessCodesQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpGet]
        public Task<ActionResult<GetAccessCodesBatchReportQuery.Result>> GetAccessCodesBatchReport([FromQuery] GetAccessCodesBatchReportQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<GenerateAccessCodesCommand.Result>> GenerateAccessCodes([FromBody] GenerateAccessCodesCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UpdateAccessCodeBatchExpiryDateCommand.Result>> UpdateAccessCodeBatchExpiryDate([FromBody] UpdateAccessCodeBatchExpiryDateCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteAccessCodesCommand.Result>> DeleteAccessCodes([FromBody] DeleteAccessCodesCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
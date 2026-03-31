using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Vouchers;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VouchersController : Controller
    {
        private readonly IMediator _mediator;

        public VouchersController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet]
        public Task<ActionResult<List<GetVouchersQuery.Result>>> GetVouchers([FromQuery] GetVouchersQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<CreateVoucherCommand.Result>> CreateVoucher([FromBody] CreateVoucherCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteVoucherCommand.Result>> DeleteVoucher([FromBody] DeleteVoucherCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
using System.Threading.Tasks;
using Biobrain.Application.WhatsNew.CreateWhatsNew;
using Biobrain.Application.WhatsNew.GetLatestWhatsNew;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WhatsNewController : Controller
    {
        private readonly IMediator _mediator;

        public WhatsNewController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet]
        public Task<ActionResult<GetLatestWhatsNewQuery.Result>> GetLatest([FromQuery] GetLatestWhatsNewQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpPost]
        public Task<ActionResult<CreateWhatsNewCommand.Result>> Create([FromBody] CreateWhatsNewCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}

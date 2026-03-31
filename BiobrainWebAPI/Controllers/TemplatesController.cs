using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Templates;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TemplatesController : Controller
    {
        private readonly IMediator _mediator;

        public TemplatesController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet]
        public Task<ActionResult<List<GetTemplatesQuery.Result>>> GetTemplates([FromQuery] GetTemplatesQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SaveTemplateCommand.Result>> SaveTemplate([FromBody] SaveTemplateCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteTemplateCommand.Result>> DeleteTemplate([FromBody] DeleteTemplateCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
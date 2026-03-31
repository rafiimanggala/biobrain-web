using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Curricula.GetCurricula;
using Biobrain.Application.Curricula.GetCurriculaWithCountryRelations;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurriculaController : Controller
    {
        private readonly IMediator _mediator;

        public CurriculaController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<List<GetCurriculaQuery.Result>>> GetCurricula([FromQuery] GetCurriculaQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetCurriculaWithCountryRelationsQuery.Result>>> GetCurriculaWithCountryRelation([FromQuery] GetCurriculaWithCountryRelationsQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}
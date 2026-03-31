using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Subjects.GetSubjects;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubjectsController : Controller
    {
        private readonly IMediator _mediator;

        public SubjectsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<List<GetSubjectsQuery.Result>>> GetSubjects([FromQuery] GetSubjectsQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}
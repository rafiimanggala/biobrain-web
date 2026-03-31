using System.Threading.Tasks;
using Biobrain.Application.AssignedWork.GetAssignedWork;
using Biobrain.Application.AssignedWork.GetTeacherAssignedWork;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AssignedWorkController : Controller
    {
        private readonly IMediator _mediator;

        public AssignedWorkController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<GetAssignedWorkQuery.Result>> GetAssignedWork([FromQuery] GetAssignedWorkQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetTeacherAssignedWorkQuery.Result>> GetTeacherAssignedWork([FromQuery] GetTeacherAssignedWorkQuery command)
            => _mediator.Send(command).ToActionResult();
    }
}

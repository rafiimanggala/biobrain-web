using System.Threading.Tasks;
using Biobrain.Application.LearningMaterialAssignments.AssignLearningMaterialToClass;
using Biobrain.Application.LearningMaterialAssignments.GetLearningMaterialUserAssignment;
using Biobrain.Application.LearningMaterialAssignments.ReassignLearningMaterialsToStudents;
using Biobrain.Application.LearningMaterialAssignments.SetAssignedLearningMaterialAsDone;
using Biobrain.Application.LearningMaterialAssignments.UnassignLearningMaterialToClass;
using Biobrain.Application.LearningMaterialAssignments.UpdateDueDateForLearningMaterialAssignment;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LearningMaterialAssignmentsController : Controller
    {
        private readonly IMediator _mediator;

        public LearningMaterialAssignmentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<AssignLearningMaterialToClassCommand.Result>> AssignLearningMaterialToClass([FromBody] AssignLearningMaterialToClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<ReassignLearningMaterialsToStudentsCommand.Result>> ReassignLearningMaterialsToStudents(
            [FromBody] ReassignLearningMaterialsToStudentsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SetAssignedLearningMaterialAsDoneCommand.Result>> SetAssignedLearningMaterialAsDone(
            [FromBody] SetAssignedLearningMaterialAsDoneCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateDueDateForLearningMaterialAssignmentCommand.Result>> UpdateDueDateForLearningMaterialAssignment(
            [FromBody] UpdateDueDateForLearningMaterialAssignmentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UnassignLearningMaterialToClass.Result>> UnassignLearningMaterialToClass(
            [FromBody] UnassignLearningMaterialToClass command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetLearningMaterialUserAssignmentQuery.Result>> GetLearningMaterialUserAssignment(
            [FromQuery] GetLearningMaterialUserAssignmentQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}
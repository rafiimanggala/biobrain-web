using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Teachers.AddTeacherByEmail;
using Biobrain.Application.Teachers.CreateTeacher;
using Biobrain.Application.Teachers.DeleteTeacher;
using Biobrain.Application.Teachers.GetSchoolTeachers;
using Biobrain.Application.Teachers.GetTeacherById;
using Biobrain.Application.Teachers.GetTeacherClasses;
using Biobrain.Application.Teachers.GetTeacherListItems;
using Biobrain.Application.Teachers.UpdateTeacherClasses;
using Biobrain.Application.Teachers.UpdateTeacherDetails;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TeachersController : Controller
    {
        private readonly IMediator _mediator;

        public TeachersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<CreateTeacherCommand.Result>> CreateTeacher(CreateTeacherCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateTeacherDetailsCommand.Result>> UpdateTeacherDetails(UpdateTeacherDetailsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteTeacherFromSchoolCommand.Result>> DeleteTeacher(DeleteTeacherFromSchoolCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AddTeacherByEmailCommand.Result>> AddTeacherByEmail(AddTeacherByEmailCommand command)
            => _mediator.Send(command).ToActionResult();


        [HttpGet]
        public Task<ActionResult<GetTeacherByIdQuery.Result>> GetById([FromQuery] GetTeacherByIdQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetSchoolTeachersListQuery.Result>>> GetSchoolTeachers([FromQuery] GetSchoolTeachersListQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetTeacherListItemsQuery.Result>>> GetAsListItems([FromQuery] GetTeacherListItemsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetTeacherClassesQuery.Result>>> GetTeacherClasses([FromQuery] GetTeacherClassesQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateTeacherClassesCommand.Result>> UpdateTeacherClasses(UpdateTeacherClassesCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
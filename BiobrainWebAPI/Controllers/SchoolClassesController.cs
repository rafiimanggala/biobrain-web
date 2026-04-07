using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.SchoolClasses.AddTeacherToSchoolClass;
using Biobrain.Application.SchoolClasses.CreateSchoolClass;
using Biobrain.Application.SchoolClasses.DeleteSchoolClass;
using Biobrain.Application.SchoolClasses.DeleteTeacherFromSchoolClass;
using Biobrain.Application.SchoolClasses.EmailSchoolClass;
using Biobrain.Application.SchoolClasses.GetSchoolClassByCourseIdQuery;
using Biobrain.Application.SchoolClasses.GetSchoolClassById;
using Biobrain.Application.SchoolClasses.GetSchoolClasses;
using Biobrain.Application.SchoolClasses.GetSchoolClassListItems;
using Biobrain.Application.SchoolClasses.InviteByEmailToSchoolClass;
using Biobrain.Application.SchoolClasses.RenameSchoolClass;
using Biobrain.Application.SchoolClasses.UpdateClassSettings;
using Biobrain.Application.SchoolClasses.UpdateSchoolClass;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SchoolClassesController : Controller
    {
        private readonly IMediator _mediator;

        public SchoolClassesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<CreateSchoolClassCommand.Result>> CreateSchoolClass(CreateSchoolClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateSchoolClassCommand.Result>> UpdateSchoolClass(UpdateSchoolClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateClassSettingsCommand.Result>> UpdateClassSettings(UpdateClassSettingsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AddTeacherToSchoolClassCommand.Result>> AddTeacherToSchoolClass(AddTeacherToSchoolClassCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteTeacherFromSchoolClassCommand.Result>> DeleteTeacherFromSchoolClass(DeleteTeacherFromSchoolClassCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<RenameSchoolClassCommand.Result>> RenameSchoolClass(RenameSchoolClassCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteSchoolClassCommand.Result>> DeleteSchoolClass(DeleteSchoolClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetSchoolClassesListQuery.Result>>> GetSchoolClasses([FromQuery] GetSchoolClassesListQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetSchoolClassByIdQuery.Result>> GetById([FromQuery] GetSchoolClassByIdQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetTeacherSchoolClassesByCourseIdQuery.Result>>> GetForTeacherByCourseId([FromQuery] GetTeacherSchoolClassesByCourseIdQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetSchoolClassListItemsQuery.Result>>> GetAsListItems([FromQuery] GetSchoolClassListItemsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<EmailSchoolClassCommand.Result>> EmailSchoolClass(EmailSchoolClassCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<InviteByEmailToSchoolClassCommand.Result>> InviteStudentByEmail(InviteByEmailToSchoolClassCommand command)
	        => _mediator.Send(command).ToActionResult();

        //[HttpPost]
        //public Task<ActionResult<InviteByEmailToSchoolClassCommand.Result>> InviteStudentByEmail(InviteByEmailToSchoolClassCommand command)
	       // => _mediator.Send(command).ToActionResult();
    }
}
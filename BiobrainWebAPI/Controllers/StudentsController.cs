using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Students.AddExistingStudentToSchool;
using Biobrain.Application.Students.CreateStudent;
using Biobrain.Application.Students.DeleteStudent;
using Biobrain.Application.Students.GetSchoolClassStudents;
using Biobrain.Application.Students.GetSchoolStudents;
using Biobrain.Application.Students.GetStudentById;
using Biobrain.Application.Students.GetStudentClasses;
using Biobrain.Application.Students.GetStudents;
using Biobrain.Application.Students.GetStudentsListItems;
using Biobrain.Application.Students.ImportStudents;
using Biobrain.Application.Students.RemoveStudentClasses;
using Biobrain.Application.Students.UpdateStudent;
using Biobrain.Application.Students.UpdateStudentClasses;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<CreateStudentCommand.Result>> CreateStudent(CreateStudentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AddExistingStudentToSchoolCommand.Result>> AddExistingStudentToSchool(AddExistingStudentToSchoolCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateStudentCommand.Result>> UpdateStudent(UpdateStudentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteStudentFromSchoolCommand.Result>> DeleteStudent(DeleteStudentFromSchoolCommand command)
            => _mediator.Send(command).ToActionResult();
        
        [HttpPost]
        public Task<ActionResult<ImportStudentsCommand.Result>> ImportStudents(ImportStudentsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetSchoolStudentsListQuery.Result>>> GetSchoolStudents([FromQuery] GetSchoolStudentsListQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetStudentsListQuery.Result>>> GetStudents([FromQuery] GetStudentsListQuery query)
            => _mediator.Send(query).ToActionResult(); 

         [HttpGet]
        public Task<ActionResult<List<GetSchoolClassStudentsListQuery.Result>>> GetSchoolClassStudents([FromQuery] GetSchoolClassStudentsListQuery query)
	        => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetStudentByIdQuery.Result>> GetById([FromQuery] GetStudentByIdQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetStudentListItemsQuery.Result>>> GetAsListItems([FromQuery] GetStudentListItemsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetStudentClassesQuery.Result>>> GetStudentClasses([FromQuery] GetStudentClassesQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateStudentClassesCommand.Result>> UpdateStudentClasses(UpdateStudentClassesCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<RemoveStudentClassCommand.Result>> RemoveStudentClass(RemoveStudentClassCommand command)
	        => _mediator.Send(command).ToActionResult();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Courses.GetCourses;
using Biobrain.Application.Courses.GetCoursesForSchool;
using Biobrain.Application.Courses.GetCoursesForStudent;
using Biobrain.Application.Courses.GetCoursesForTeacher;
using Biobrain.Application.Courses.ImportCourses;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoursesController : Controller
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<List<GetAllCoursesQuery.Result>>> GetCourses([FromQuery] GetAllCoursesQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpGet]
        public Task<ActionResult<List<GetCoursesForSchoolQuery.Result>>> GetCoursesForSchool([FromQuery] GetCoursesForSchoolQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpPost]
        public Task<ActionResult<List<GetCoursesForStudentQuery.Result>>> GetCoursesForStudent([FromBody] GetCoursesForStudentQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpGet]
        public Task<ActionResult<List<GetCoursesForTeacherQuery.Result>>> GetCoursesForTeacher([FromQuery] GetCoursesForTeacherQuery query)
            => _mediator.Send(query).ToActionResult();

        [Authorize]
        [HttpPost]
        public Task<ActionResult<ImportCoursesCommand.Result>> ImportCourses([FromBody] ImportCoursesCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
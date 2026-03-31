using System.Threading.Tasks;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Application.Quizzes.GetQuizCompletenessStatus;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuizzesAnalyticController : Controller
    {
        private readonly IMediator _mediator;

        public QuizzesAnalyticController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<GetClassResultsQuery.Result>> GetClassResults([FromBody] GetClassResultsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetClassResultsCsvQuery.Result>> GetClassResultsCsv([FromQuery] GetClassResultsCsvQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetQuizAssignmentResultQuery.Result>> GetQuizAssignmentResult([FromQuery] GetQuizAssignmentResultQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetStudentQuizAssignmentResultsQuery.Result>> GetStudentQuizAssignmentResults([FromQuery] GetStudentQuizAssignmentResultsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetQuizFullnessStatusQuery.Result>> GetQuizFullnessStatus([FromQuery] GetQuizFullnessStatusQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}

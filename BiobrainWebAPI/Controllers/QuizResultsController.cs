using System.Threading.Tasks;
using Biobrain.Application.Quizzes.EnsureQuizResultForAssignment;
using Biobrain.Application.Quizzes.GenerateSubsectionQuiz;
using Biobrain.Application.Quizzes.GenerateTopicQuiz;
using Biobrain.Application.Quizzes.GetLastIndividualUncompletedQuizResult;
using Biobrain.Application.Quizzes.GetQuizResult;
using Biobrain.Application.Quizzes.GetQuizResultForLevel;
using Biobrain.Application.Quizzes.GetQuizResultHistory;
using Biobrain.Application.Quizzes.GetQuizResultStreak;
using Biobrain.Application.Quizzes.SetQuizResultQuestionValue;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuizResultsController : Controller
    {
        private readonly IMediator _mediator;

        public QuizResultsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<GetLastIndividualUncompletedQuizResultQuery.Result>> GetLastIndividualUncompletedQuizResult([FromQuery] GetLastIndividualUncompletedQuizResultQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetQuizResultHistoryQuery.Result>> GetQuizResultHistory([FromQuery] GetQuizResultHistoryQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GetQuizResultForLevelQuery.Result>> GetQuizResultForLevel(GetQuizResultForLevelQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetQuizResultQuery.Result>> GetQuizResult([FromQuery] GetQuizResultQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GetQuizResultStreakCommand.Result>> GetQuizResultStreak(GetQuizResultStreakCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SetQuizResultQuestionValueCommand.Result>> SetQuizResultQuestionValue([FromBody] SetQuizResultQuestionValueCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<EnsureQuizResultForAssignmentCommand.Result>> EnsureQuizResultForAssignment([FromBody] EnsureQuizResultForAssignmentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GenerateTopicQuizCommand.Result>> GenerateTopicQuiz([FromBody] GenerateTopicQuizCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<GenerateSubsectionQuizCommand.Result>> GenerateSubsectionQuiz([FromBody] GenerateSubsectionQuizCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}

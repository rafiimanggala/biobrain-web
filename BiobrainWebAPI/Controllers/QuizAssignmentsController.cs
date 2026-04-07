using System.Threading.Tasks;
using Biobrain.Application.Quizzes.AssignUserToIndividualQuiz;
using Biobrain.Application.Quizzes.CreateQuizFromTemplate;
using Biobrain.Application.Quizzes.CreateStudentCustomQuiz;
using Biobrain.Application.Quizzes.CreateTeacherCustomQuiz;
using Biobrain.Application.Quizzes.GetQuizTemplates;
using Biobrain.Application.Quizzes.GetStudentCustomQuizzes;
using Biobrain.Application.Quizzes.RetakeStudentCustomQuiz;
using Biobrain.Application.Quizzes.Perform.AssignQuizzesToClass;
using Biobrain.Application.Quizzes.Perform.ExcludedQuestions;
using Biobrain.Application.Quizzes.Perform.ReassignQuizzesToStudents;
using Biobrain.Application.Quizzes.Perform.ToggleQuizHints;
using Biobrain.Application.Quizzes.Perform.UnassignQuizzesToClass;
using Biobrain.Application.Quizzes.Perform.UpdateDueDateForQuizAssignment;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuizAssignmentsController : Controller
    {
        private readonly IMediator _mediator;

        public QuizAssignmentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<GetExcludedQuestionsCommand.Result>> GetExcludedQuestions([FromQuery] GetExcludedQuestionsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AssignQuizzesToClassCommand.Result>> AssignQuizzesToClass([FromBody] AssignQuizzesToClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<ReassignQuizzesToStudentsCommand.Result>> ReassignQuizzesToStudents([FromBody] ReassignQuizzesToStudentsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateDueDateForQuizAssignmentCommand.Result>> UpdateDueDateForQuizAssignment(
            [FromBody] UpdateDueDateForQuizAssignmentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UnassignQuizToClassCommand.Result>> UnassignQuizzToClass(
            [FromBody] UnassignQuizToClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AssignUserToIndividualQuizCommand.Result>> AssignUserToIndividualQuiz([FromBody] AssignUserToIndividualQuizCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AddExcludedQuestionCommand.Result>> AddExcludedQuestion([FromBody] AddExcludedQuestionCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<CreateStudentCustomQuizCommand.Result>> CreateStudentCustomQuiz([FromBody] CreateStudentCustomQuizCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetStudentCustomQuizzesQuery.Result>> GetStudentCustomQuizzes([FromQuery] GetStudentCustomQuizzesQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        public Task<ActionResult<RetakeStudentCustomQuizCommand.Result>> RetakeStudentCustomQuiz([FromBody] RetakeStudentCustomQuizCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<CreateTeacherCustomQuizCommand.Result>> CreateTeacherCustomQuiz([FromBody] CreateTeacherCustomQuizCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<CreateQuizFromTemplateCommand.Result>> CreateQuizFromTemplate([FromBody] CreateQuizFromTemplateCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<ToggleQuizHintsCommand.Result>> ToggleQuizHints([FromBody] ToggleQuizHintsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetQuizTemplatesQuery.Result>> GetQuizTemplates([FromQuery] GetQuizTemplatesQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}

using System.Threading.Tasks;
using Biobrain.Application.Feedback;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<SubmitFeedbackCommand.Result>> SubmitFeedback(SubmitFeedbackCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}

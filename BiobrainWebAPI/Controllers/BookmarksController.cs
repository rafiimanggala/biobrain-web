using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Bookmarks.CreateBookmarkForUser;
using Biobrain.Application.Bookmarks.DeleteBookmarkForUser;
using Biobrain.Application.Bookmarks.GetBookmarksForUser;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly IMediator _mediator;

        public BookmarksController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public Task<ActionResult<List<GetBookmarksForUserQuery.Result>>> GetBookmarksForUser([FromQuery] GetBookmarksForUserQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<CreateBookmarkForUserCommand.Result>> CreateBookmarkForUser([FromBody] CreateBookmarkForUserCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteBookmarkForUserCommand.Result>> DeleteBookmarkForUser([FromBody] DeleteBookmarkForUserCommand command)
	        => _mediator.Send(command).ToActionResult();
    }
}

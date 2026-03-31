using System.Threading.Tasks;
using Biobrain.Application.Tracking.PageView;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Moovosity.WebApi.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserTrackingController : Controller
    {
        private readonly IMediator _mediator;

        public UserTrackingController(IMediator mediator) => _mediator = mediator;

        //[HttpGet]
        //public Task<ActionResult<TrackSessionQuery.Result>> Session([FromQuery] TrackSessionQuery query)
        //    => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<PageViewQuery.Result>> PageView([FromQuery] PageViewQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}

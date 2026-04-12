using System.Threading.Tasks;
using Biobrain.Application.Content.ContentImages;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ContentImagesController : Controller
    {
        private readonly IMediator _mediator;

        public ContentImagesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<UploadContentImageCommand.Result>> Upload(
            [FromForm] IFormFile file,
            [FromForm] string code,
            [FromForm] string description = "")
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            var command = new UploadContentImageCommand
            {
                File = file,
                Code = code,
                Description = description,
            };

            return await _mediator.Send(command).ToActionResult();
        }

        [HttpGet]
        public Task<ActionResult<GetContentImagesQuery.Result>> GetImages([FromQuery] GetContentImagesQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}

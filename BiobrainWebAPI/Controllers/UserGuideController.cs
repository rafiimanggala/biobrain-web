using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.UserGuide.DeleteUserGuideNode;
using Biobrain.Application.UserGuide.GetUserGuideContent;
using Biobrain.Application.UserGuide.GetUserGuideContentTree;
using Biobrain.Application.UserGuide.ReorderUserGuideContentTreeNode;
using Biobrain.Application.UserGuide.SaveUserGuideContent;
using Biobrain.Application.UserGuide.SaveUserGuideNode;
using Biobrain.Application.UserGuide.SendQuestion;
using Biobrain.Application.UserGuide.UploadUserGuideImageCommand;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserGuideController : Controller
    {
        private readonly IMediator _mediator;

        public UserGuideController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet]
        public Task<ActionResult<List<GetUserGuideContentTreeQuery.Result>>> GetUserGuideContentTree([FromQuery] GetUserGuideContentTreeQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SaveUserGuideNodeCommand.Result>> SaveUserGuideNode([FromBody] SaveUserGuideNodeCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteUserGuideNodeCommand.Result>> DeleteUserGuideNode([FromBody] DeleteUserGuideNodeCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ReorderUserGuideContentTreeNodeCommand.Result>> ReorderUserGuideContentTreeNode([FromBody] ReorderUserGuideContentTreeNodeCommand command)
            => _mediator.Send(command).ToActionResult();


        [Authorize]
        [HttpGet]
        public Task<ActionResult<GetUserGuideContentQuery.Result>> GetUserGuideContent([FromQuery] GetUserGuideContentQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SaveUserGuideContentCommand.Result>> SaveUserGuideContent([FromBody] SaveUserGuideContentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SendQuestionCommand.Result>> SendQuestion([FromBody] SendQuestionCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UploadUserGuideImageCommand.Result>> UploadUserGuideImage(IFormFile file)
        {
            if (file.Length < 1) throw new BadHttpRequestException("Empty file input");

            return await _mediator.Send(new UploadUserGuideImageCommand
            {
                File = file,
                FileId = Guid.NewGuid()
            }).ToActionResult();
        }
    }
}
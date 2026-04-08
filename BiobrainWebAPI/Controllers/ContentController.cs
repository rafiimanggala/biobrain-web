using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Biobrain.Application.Content.AddAllNewQuestions;
using Biobrain.Application.Content.AttachMaterialToNode;
using Biobrain.Application.Content.AttachQuestionToNode;
using Biobrain.Application.Content.CopyTopic;
using Biobrain.Application.Content.CreateContentTreeNode;
using Biobrain.Application.Content.CreateMaterial;
using Biobrain.Application.Content.CreateQuestion;
using Biobrain.Application.Content.UpdateMaterial;
using Biobrain.Application.Content.UpdateQuestion;
using Biobrain.Application.Content.DeleteContentTreeNode;
using Biobrain.Application.Content.GetBaseMaterials;
using Biobrain.Application.Content.GetBaseQuestions;
using Biobrain.Application.Content.GetActualContentVersion;
using Biobrain.Application.Content.GetContentData;
using Biobrain.Application.Content.GetContentTree;
using Biobrain.Application.Content.GetContentTreeMeta;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Content.GetCourseContentData;
using Biobrain.Application.Content.GetCourseReleases;
using Biobrain.Application.Content.GetCourses;
using Biobrain.Application.Content.GetPage;
using Biobrain.Application.Content.GetQuiz;
using Biobrain.Application.Content.GetQuizById;
using Biobrain.Application.Content.ImportContent;
using Biobrain.Application.Content.ImportContentFromJson;
using Biobrain.Application.Content.IncrementContentVersion;
using Biobrain.Application.Content.SwitchOrderForNodes;
using Biobrain.Application.Content.UpdateContentTreeNode;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Biobrain.Application.Content.GetCourseQuizzesList;
using Biobrain.Application.Content.UpdateAutoMapOptions;
using Biobrain.Application.Content.ExcludeQuestion;
using Biobrain.Application.Content.IncludeQuestion;
using Biobrain.Application.Content.GetAutoMapOptions;
using Biobrain.Application.Content.DisableAutoMap;
using Biobrain.Application.Scripts;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly IMediator _mediator;

        public ContentController(IMediator mediator) => _mediator = mediator;

        // Import
        [HttpPost]
        [Authorize]
        public Task<ActionResult<ImportContentFromJsonCommand.Result>> Import(IFormFile file)
        {
	        if (file.Length < 1) throw new BadHttpRequestException("Empty file input");
	        var text = new StreamReader(file.OpenReadStream()).ReadToEnd();

	        var command = JsonConvert.DeserializeObject<ImportContentFromJsonCommand>(text);
            if(command == null) throw new BadHttpRequestException("Can't deserialize file");

            return _mediator.Send(command).ToActionResult();
        }

        // ImportContent (web content import)
        [HttpPost]
        [Authorize]
        public Task<ActionResult<ImportContentCommand.Result>> ImportContent([FromBody] ImportContentCommand command)
            => _mediator.Send(command).ToActionResult();

        // IncrementVersion
        [HttpPost]
        [Authorize]
        public Task<ActionResult<IncrementContentVersionCommand.Result>> IncrementVersion(IncrementContentVersionCommand command)
            => _mediator.Send(command).ToActionResult();
        // IncrementVersion

        [HttpPost]
        public Task<ActionResult<List<AddAllNewQuestionsCommand.Result>>> AddAllNewQuestions([FromBody] AddAllNewQuestionsCommand command)
	        => _mediator.Send(command).ToActionResult();

        // 
        //[HttpPost]
        //public Task<ActionResult<DeleteDeletedCommand.Result>> DeleteDeletedForPhysics(DeleteDeletedCommand command)
        //	=> _mediator.Send(command).ToActionResult();

        //// 
        //[HttpPost]
        //public Task<ActionResult<DeleteNodesAttachedForChemistryCommand.Result>> DeleteNodesAttachedForChemistry(DeleteNodesAttachedForChemistryCommand command)
        // => _mediator.Send(command).ToActionResult();

        // Tree
        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetCoursesQuery.Result>>> GetCourses([FromQuery] GetCoursesQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetCourseReleasesQuery.Result>>> GetCourseReleases([FromQuery] GetCourseReleasesQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetContentTreeMetaByCourseIdQuery.Result>>> GetCourseContentTreeMeta([FromQuery] GetContentTreeMetaByCourseIdQuery command)
            => _mediator.Send(command).ToActionResult();

        // Nodes
        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetContentTreeByCourseIdQuery.Result>>> GetCourseContentTree([FromQuery] GetContentTreeByCourseIdQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetCourseQuizzesListQuery.Result>>> GetCourseQuizzesList([FromQuery] GetCourseQuizzesListQuery command)
            => _mediator.Send(command).ToActionResult();
        
        [HttpPost]
        [Authorize]
        public Task<ActionResult<List<CreateContentTreeNodeCommand.Result>>> CreateContentTreeNode([FromBody] CreateContentTreeNodeCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UpdateContentTreeNodeCommand.Result>> UpdateContentTreeNode([FromBody] UpdateContentTreeNodeCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SwitchOrderForNodesCommand.Result>> SwitchOrderForNodes([FromBody] SwitchOrderForNodesCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteContentTreeNodeCommand.Result>> DeleteContentTreeNode([FromBody] DeleteContentTreeNodeCommand command)
	        => _mediator.Send(command).ToActionResult();

        // Content
        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetPageByNodeIdQuery.Result>>> GetNodePage([FromQuery] GetPageByNodeIdQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetQuizByNodeIdQuery.Result>>> GetNodeQuiz([FromQuery] GetQuizByNodeIdQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<ContentData.Quiz>> GetQuizById([FromQuery] GetQuizByIdQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetBaseMaterialsQuery.Result>>> GetBaseMaterials([FromQuery] GetBaseMaterialsQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetBaseQuestionsQuery.Result>>> GetBaseQuestions([FromQuery] GetBaseQuestionsQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<GetAutoMapOptionsQuery.Result>> GetAutoMapOptions([FromQuery] GetAutoMapOptionsQuery command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<AttachMaterialsToNodeCommand.Result>> AttachMaterialsToNode([FromBody] AttachMaterialsToNodeCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<AttachQuestionToNode.Result>> AttachQuestionsToNode([FromBody] AttachQuestionToNode command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<CreateMaterialCommand.Result>> CreateMaterial([FromBody] CreateMaterialCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<CreateQuestionCommand.Result>> CreateQuestion([FromBody] CreateQuestionCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UpdateMaterialCommand.Result>> UpdateMaterial([FromBody] UpdateMaterialCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UpdateQuestionCommand.Result>> UpdateQuestion([FromBody] UpdateQuestionCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DisableAutoMapCommand.Result>> DisableAutoMap([FromBody] DisableAutoMapCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ExcludeQuestionFromQuizAutoMap.Result>> ExcludeQuestionFromQuizAutoMap([FromBody] ExcludeQuestionFromQuizAutoMap command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<IncludeQuestionToQuizAutoMap.Result>> IncludeQuestionToQuizAutoMap([FromBody] IncludeQuestionToQuizAutoMap command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UpdateAutoMapOptionsCommand.Result>> UpdateAutoMapOptions([FromBody] UpdateAutoMapOptionsCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<List<CopyNodeCommand.Result>>> CopyNode([FromBody] CopyNodeCommand command)
	        => _mediator.Send(command).ToActionResult();



		[HttpGet]
        [Authorize]
        [Obsolete]
        public Task<ActionResult<GetContentDataQuery.Result>> GetContentData([FromQuery] GetContentDataQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<GetCourseContentDataQuery.Result>> GetCourseContentData([FromQuery] GetCourseContentDataQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        [Authorize]
        public Task<ActionResult<List<GetActualContentVersionQuery.Result>>> GetActualContentVersion([FromQuery] GetActualContentVersionQuery query)
            => _mediator.Send(query).ToActionResult();



        [HttpPost]
        [Authorize]
        public Task<ActionResult<MapCoursesCommand.Result>> MapCourses([FromBody] MapCoursesCommand command)
            => _mediator.Send(command).ToActionResult();
    }
}
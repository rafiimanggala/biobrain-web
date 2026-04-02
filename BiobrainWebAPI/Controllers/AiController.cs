using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AiController : Controller
    {
        private readonly IPerformanceInsightsService _insightsService;
        private readonly IAskBiobrainService _askBiobrainService;
        private readonly IPracticeSetGeneratorService _practiceSetGeneratorService;

        public AiController(
            IPerformanceInsightsService insightsService,
            IAskBiobrainService askBiobrainService,
            IPracticeSetGeneratorService practiceSetGeneratorService)
        {
            _insightsService = insightsService;
            _askBiobrainService = askBiobrainService;
            _practiceSetGeneratorService = practiceSetGeneratorService;
        }

        [HttpPost]
        public async Task<ActionResult<SendWeeklyInsightsResponse>> SendWeeklyInsights()
        {
            await _insightsService.SendWeeklyInsightsAsync();
            return Ok(new SendWeeklyInsightsResponse { Success = true, Message = "Weekly insights emails sent." });
        }

        [HttpPost]
        public async Task<ActionResult<PreviewInsightsResponse>> PreviewInsights(
            [FromBody] PreviewInsightsRequest request)
        {
            if (request == null
                || request.SchoolClassId == Guid.Empty
                || request.CourseId == Guid.Empty)
            {
                return BadRequest("SchoolClassId and CourseId are required.");
            }

            var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-7);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            var insightsHtml = await _insightsService.GenerateInsightsForClassAsync(
                request.SchoolClassId, request.CourseId, fromDate, toDate);

            return Ok(new PreviewInsightsResponse
            {
                SchoolClassId = request.SchoolClassId,
                CourseId = request.CourseId,
                FromDate = fromDate,
                ToDate = toDate,
                InsightsHtml = insightsHtml
            });
        }

        [HttpPost]
        public async Task<ActionResult<AskBiobrainResponse>> Ask(
            [FromBody] AskBiobrainRequest request)
        {
            if (request == null
                || request.CourseId == Guid.Empty
                || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("CourseId and Question are required.");
            }

            var answer = await _askBiobrainService.AskAsync(
                request.CourseId,
                request.ContentTreeNodeId,
                request.Question,
                request.ConversationHistory ?? new List<ChatMessage>());

            return Ok(new AskBiobrainResponse { Answer = answer });
        }

        [HttpPost]
        public async Task<ActionResult<GeneratePracticeSetResponse>> GeneratePracticeSet(
            [FromBody] GeneratePracticeSetRequest request)
        {
            if (request == null
                || request.CourseId == Guid.Empty
                || request.ContentTreeNodeId == Guid.Empty
                || request.TeacherId == Guid.Empty)
            {
                return BadRequest("CourseId, ContentTreeNodeId, and TeacherId are required.");
            }

            var questionIds = await _practiceSetGeneratorService.GenerateAsync(
                request.CourseId,
                request.ContentTreeNodeId,
                request.QuestionCount,
                request.QuestionType,
                request.TeacherId,
                request.DifficultyLevel);

            return Ok(new GeneratePracticeSetResponse
            {
                QuestionIds = questionIds,
                Count = questionIds.Count
            });
        }
    }

    // --- Ask BioBrain DTOs ---
    public class AskBiobrainRequest
    {
        public Guid CourseId { get; set; }
        public Guid? ContentTreeNodeId { get; set; }
        public string Question { get; set; }
        public List<ChatMessage> ConversationHistory { get; set; }
    }

    public class AskBiobrainResponse
    {
        public string Answer { get; set; }
    }

    // --- Practice Set Generator DTOs ---
    public class GeneratePracticeSetRequest
    {
        public Guid CourseId { get; set; }
        public Guid ContentTreeNodeId { get; set; }
        public int QuestionCount { get; set; } = 5;
        public string QuestionType { get; set; } = "multiple_choice";
        public Guid TeacherId { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
    }

    public class GeneratePracticeSetResponse
    {
        public List<Guid> QuestionIds { get; set; }
        public int Count { get; set; }
    }

    public class PreviewInsightsRequest
    {
        public Guid SchoolClassId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class PreviewInsightsResponse
    {
        public Guid SchoolClassId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string InsightsHtml { get; set; }
    }

    public class SendWeeklyInsightsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

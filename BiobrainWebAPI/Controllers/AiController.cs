using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDb _db;

        public AiController(
            IPerformanceInsightsService insightsService,
            IAskBiobrainService askBiobrainService,
            IPracticeSetGeneratorService practiceSetGeneratorService,
            IDb db)
        {
            _insightsService = insightsService;
            _askBiobrainService = askBiobrainService;
            _practiceSetGeneratorService = practiceSetGeneratorService;
            _db = db;
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

        [HttpPost]
        public async Task<ActionResult<GetQuestionsByIdsResponse>> GetQuestionsByIds(
            [FromBody] GetQuestionsByIdsRequest request)
        {
            if (request == null || request.QuestionIds == null || request.QuestionIds.Count == 0)
            {
                return Ok(new GetQuestionsByIdsResponse { Questions = new List<QuestionDto>() });
            }

            var ids = request.QuestionIds
                .Where(s => Guid.TryParse(s, out _))
                .Select(Guid.Parse)
                .ToList();

            if (ids.Count == 0)
            {
                return Ok(new GetQuestionsByIdsResponse { Questions = new List<QuestionDto>() });
            }

            var questions = await _db.Questions
                .AsNoTracking()
                .Where(q => ids.Contains(q.QuestionId) && q.DeletedAt == null)
                .Include(q => q.QuestionType)
                .Include(q => q.Answers)
                .ToListAsync();

            var idOrder = request.QuestionIds
                .Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            var dtos = questions
                .OrderBy(q => idOrder.TryGetValue(q.QuestionId.ToString(), out var idx) ? idx : int.MaxValue)
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId.ToString(),
                    QuestionTypeCode = q.QuestionTypeCode.ToString(),
                    QuestionTypeName = q.QuestionType != null ? q.QuestionType.Name : string.Empty,
                    Header = q.Header ?? string.Empty,
                    Text = q.Text ?? string.Empty,
                    Hint = q.Hint ?? string.Empty,
                    FeedBack = q.FeedBack ?? string.Empty,
                    Answers = (q.Answers ?? new List<DataAccessLayer.WebAppEntities.AnswerEntity>())
                        .OrderBy(a => a.AnswerOrder)
                        .Select(a => new AnswerDto
                        {
                            AnswerId = a.AnswerId.ToString(),
                            AnswerOrder = a.AnswerOrder,
                            Text = a.Text ?? string.Empty,
                            IsCorrect = a.IsCorrect,
                            CaseSensitive = a.CaseSensitive,
                            Score = a.Score,
                            Response = a.Response
                        })
                        .ToList()
                })
                .ToList();

            return Ok(new GetQuestionsByIdsResponse { Questions = dtos });
        }
    }

    // --- Get Questions By Ids DTOs ---
    public class GetQuestionsByIdsRequest
    {
        public List<string> QuestionIds { get; set; }
    }

    public class GetQuestionsByIdsResponse
    {
        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public string QuestionId { get; set; }
        public string QuestionTypeCode { get; set; }
        public string QuestionTypeName { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public string FeedBack { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public string AnswerId { get; set; }
        public int AnswerOrder { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public bool CaseSensitive { get; set; }
        public int Score { get; set; }
        public int Response { get; set; }
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

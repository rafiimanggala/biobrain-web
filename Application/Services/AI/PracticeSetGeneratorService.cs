using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Entities.Question;
using DataAccessLayer.WebAppEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.AI
{
    public interface IPracticeSetGeneratorService
    {
        Task<List<Guid>> GenerateAsync(
            Guid courseId,
            Guid contentTreeNodeId,
            int questionCount,
            string questionType,
            Guid teacherId);
    }

    public class PracticeSetGeneratorService : IPracticeSetGeneratorService
    {
        private readonly IAiService _aiService;
        private readonly IDb _db;
        private readonly ILogger<PracticeSetGeneratorService> _logger;

        public PracticeSetGeneratorService(
            IAiService aiService,
            IDb db,
            ILogger<PracticeSetGeneratorService> logger)
        {
            _aiService = aiService;
            _db = db;
            _logger = logger;
        }

        public async Task<List<Guid>> GenerateAsync(
            Guid courseId,
            Guid contentTreeNodeId,
            int questionCount,
            string questionType,
            Guid teacherId)
        {
            if (questionCount < 1 || questionCount > 50)
            {
                throw new ArgumentException("Question count must be between 1 and 50.", nameof(questionCount));
            }

            var courseInfo = await _db.Courses
                .AsNoTracking()
                .Include(c => c.Subject)
                .Where(c => c.CourseId == courseId)
                .Select(c => new { c.CourseId, SubjectName = c.Subject.Name })
                .FirstOrDefaultAsync();

            if (courseInfo == null)
            {
                throw new ArgumentException($"Course {courseId} not found.", nameof(courseId));
            }

            var topicName = await _db.ContentTree
                .AsNoTracking()
                .Where(ct => ct.NodeId == contentTreeNodeId && ct.CourseId == courseId)
                .Select(ct => ct.Name)
                .FirstOrDefaultAsync();

            if (topicName == null)
            {
                throw new ArgumentException(
                    $"Content tree node {contentTreeNodeId} not found in course {courseId}.",
                    nameof(contentTreeNodeId));
            }

            var questionTypeEntity = await ResolveQuestionTypeAsync(questionType);
            var systemPrompt = BuildSystemPrompt(courseInfo.SubjectName, topicName, questionType);
            var userPrompt = BuildUserPrompt(questionCount, questionType, topicName, courseInfo.SubjectName);

            string aiResponse;
            try
            {
                aiResponse = await _aiService.GenerateAsync(systemPrompt, userPrompt, 4096);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI generation failed for course {CourseId}, topic {TopicName}", courseId, topicName);
                throw;
            }

            var generatedQuestions = ParseAiResponse(aiResponse);
            if (generatedQuestions == null || !generatedQuestions.Any())
            {
                throw new InvalidOperationException("AI returned no valid questions. Please try again.");
            }

            var questionIds = await SaveQuestionsAsync(generatedQuestions, courseId, questionTypeEntity.QuestionTypeCode);
            return questionIds;
        }

        private async Task<QuestionTypeEntity> ResolveQuestionTypeAsync(string questionType)
        {
            var normalised = (questionType ?? "multiple_choice").ToLowerInvariant().Replace(" ", "_");

            var entity = await _db.QuestionTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(qt => qt.Name.ToLower().Replace(" ", "_") == normalised);

            if (entity != null)
            {
                return entity;
            }

            // Fallback: return the first available question type
            var fallback = await _db.QuestionTypes.AsNoTracking().FirstOrDefaultAsync();
            if (fallback == null)
            {
                throw new InvalidOperationException("No question types configured in the database.");
            }

            return fallback;
        }

        private static string BuildSystemPrompt(string subjectName, string topicName, string questionType)
        {
            return $@"You are an expert {subjectName} educator and question writer for BioBrain,
an educational platform. Generate high-quality {questionType} questions about ""{topicName}"".

Rules:
- Questions must be factually accurate and curriculum-appropriate
- Each question must have clear, unambiguous wording
- Provide exactly one correct answer per question
- Distractors (wrong answers) should be plausible but clearly incorrect
- Include a helpful hint that guides without giving away the answer
- Include feedback that explains why the correct answer is right
- Vary difficulty levels across the set

IMPORTANT: Respond ONLY with a valid JSON array, no additional text or markdown.";
        }

        private static string BuildUserPrompt(
            int questionCount, string questionType, string topicName, string subjectName)
        {
            return $@"Generate exactly {questionCount} {questionType} questions about ""{topicName}"" in {subjectName}.

Return a JSON array with this exact structure:
[
  {{
    ""header"": ""Short question title"",
    ""text"": ""Full question text"",
    ""hint"": ""A helpful hint"",
    ""feedback"": ""Explanation of the correct answer"",
    ""answers"": [
      {{ ""text"": ""Answer option A"", ""isCorrect"": true }},
      {{ ""text"": ""Answer option B"", ""isCorrect"": false }},
      {{ ""text"": ""Answer option C"", ""isCorrect"": false }},
      {{ ""text"": ""Answer option D"", ""isCorrect"": false }}
    ]
  }}
]

Each question should have 4 answer options with exactly 1 correct answer.
Respond ONLY with the JSON array.";
        }

        private List<GeneratedQuestion> ParseAiResponse(string aiResponse)
        {
            // Strip markdown code fences if present
            var json = aiResponse.Trim();
            if (json.StartsWith("```"))
            {
                var firstNewline = json.IndexOf('\n');
                if (firstNewline >= 0)
                {
                    json = json.Substring(firstNewline + 1);
                }

                if (json.EndsWith("```"))
                {
                    json = json.Substring(0, json.Length - 3);
                }

                json = json.Trim();
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<GeneratedQuestion>>(json, options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse AI response as JSON: {Response}", aiResponse);
                throw new InvalidOperationException("AI returned invalid JSON. Please try again.", ex);
            }
        }

        private async Task<List<Guid>> SaveQuestionsAsync(
            List<GeneratedQuestion> generated,
            Guid courseId,
            long questionTypeCode)
        {
            var questionIds = new List<Guid>();

            foreach (var gq in generated)
            {
                var questionId = Guid.NewGuid();

                var question = new QuestionEntity
                {
                    QuestionId = questionId,
                    CourseId = courseId,
                    QuestionTypeCode = questionTypeCode,
                    Header = gq.Header ?? "",
                    Text = gq.Text ?? "",
                    Hint = gq.Hint ?? "",
                    FeedBack = gq.Feedback ?? "",
                    Answers = (gq.Answers ?? new List<GeneratedAnswer>())
                        .Select((a, index) => new AnswerEntity
                        {
                            AnswerId = Guid.NewGuid(),
                            QuestionId = questionId,
                            CourseId = courseId,
                            AnswerOrder = index + 1,
                            Text = a.Text ?? "",
                            IsCorrect = a.IsCorrect,
                            CaseSensitive = false,
                            Score = a.IsCorrect ? 1 : 0,
                            Response = 0
                        })
                        .ToList()
                };

                _db.Questions.Add(question);
                questionIds.Add(questionId);
            }

            await _db.SaveChangesAsync();
            return questionIds;
        }

        private class GeneratedQuestion
        {
            public string Header { get; set; }
            public string Text { get; set; }
            public string Hint { get; set; }
            public string Feedback { get; set; }
            public List<GeneratedAnswer> Answers { get; set; }
        }

        private class GeneratedAnswer
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.AI
{
    public interface IAskBiobrainService
    {
        Task<string> AskAsync(
            Guid courseId,
            Guid? contentTreeNodeId,
            string question,
            List<ChatMessage> conversationHistory);
    }

    public class AskBiobrainService : IAskBiobrainService
    {
        private readonly IAiService _aiService;
        private readonly IDb _db;
        private readonly ILogger<AskBiobrainService> _logger;

        public AskBiobrainService(
            IAiService aiService,
            IDb db,
            ILogger<AskBiobrainService> logger)
        {
            _aiService = aiService;
            _db = db;
            _logger = logger;
        }

        public async Task<string> AskAsync(
            Guid courseId,
            Guid? contentTreeNodeId,
            string question,
            List<ChatMessage> conversationHistory)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty.", nameof(question));
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

            string topicName = null;
            if (contentTreeNodeId.HasValue)
            {
                topicName = await _db.ContentTree
                    .AsNoTracking()
                    .Where(ct => ct.NodeId == contentTreeNodeId.Value && ct.CourseId == courseId)
                    .Select(ct => ct.Name)
                    .FirstOrDefaultAsync();
            }

            var systemPrompt = BuildSystemPrompt(courseInfo.SubjectName, topicName);

            var messages = new List<ChatMessage>();
            if (conversationHistory != null)
            {
                messages.AddRange(conversationHistory);
            }

            messages.Add(new ChatMessage
            {
                Role = "user",
                Content = question
            });

            try
            {
                var answer = await _aiService.ChatAsync(systemPrompt, messages, 1024);
                return answer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate chat response for course {CourseId}", courseId);
                throw;
            }
        }

        private static string BuildSystemPrompt(string subjectName, string topicName)
        {
            var topicContext = string.IsNullOrEmpty(topicName)
                ? ""
                : $" The student is currently studying the topic: \"{topicName}\".";

            return $@"You are BioBrain Tutor, a friendly and knowledgeable educational assistant
specializing in {subjectName}. Your role is to help students understand concepts clearly.{topicContext}

Guidelines:
- Explain concepts in a clear, age-appropriate way
- Use examples and analogies when helpful
- If a student is confused, try a different explanation approach
- Encourage critical thinking by asking follow-up questions when appropriate
- Keep answers concise but thorough
- If a question is outside the scope of {subjectName}, politely redirect the student
- Use simple formatting (bullet points, numbered lists) for clarity
- Never provide direct answers to assessment questions — guide the student to find the answer";
        }
    }
}

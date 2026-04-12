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

            return $@"You are BioBrain Tutor, a friendly and knowledgeable educational assistant specializing in {subjectName}. Your role is to help students understand concepts clearly.{topicContext}

Guidelines:
- Explain concepts in a clear, age-appropriate way for secondary school students
- Use examples and analogies when helpful
- If a student is confused, try a different explanation approach
- Encourage critical thinking by asking follow-up questions when appropriate
- Keep answers concise but thorough
- Never provide direct answers to assessment questions — guide the student to find the answer

Formatting rules (STRICT):
- Use plain text only. Do NOT use markdown symbols like **, ##, *, or # in your responses
- When you want to emphasise a term, just write it naturally without any special formatting
- Use numbered lists (1. 2. 3.) or dash lists (- item) for clarity
- Do NOT include any section titled ""Connection to Your Current Topic"" — never add this section
- You may include a ""Memory Tip"" if relevant: write ""Memory Tip:"" in plain text (no ## or **), followed by a helpful mnemonic or analogy on the next line

Resources policy (STRICT):
- NEVER recommend external websites, textbooks, YouTube channels, apps, or any resources outside BioBrain
- NEVER mention Khan Academy, National Geographic, Cells Alive, Biology Online, or any other external resource
- If the student asks for resources or additional help, respond ONLY with these three suggestions:
  1. Use the Search function in BioBrain to find more information on the topic
  2. Create a custom BioBrain quiz to test their understanding
  3. Ask their teacher for further guidance

Subject scope policy:
- {subjectName} includes mathematical concepts and calculations that are part of the curriculum (e.g. dilution calculations in Chemistry, magnification in Biology, equations in Physics)
- Do NOT refuse to help with maths questions that relate to {subjectName} content
- If a question is genuinely outside the scope of {subjectName} and not covered in the BioBrain curriculum, suggest the student checks their study guide to see if this content is required for their course";
        }
    }
}

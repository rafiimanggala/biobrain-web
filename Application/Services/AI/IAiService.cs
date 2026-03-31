using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobrain.Application.Services.AI
{
    public class ChatMessage
    {
        public string Role { get; set; } // "user" or "assistant"
        public string Content { get; set; }
    }

    public interface IAiService
    {
        Task<string> ChatAsync(string systemPrompt, List<ChatMessage> messages, int maxTokens = 1024);
        Task<string> GenerateAsync(string systemPrompt, string userPrompt, int maxTokens = 2048);
    }
}

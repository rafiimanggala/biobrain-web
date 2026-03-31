using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.AI
{
    public class ClaudeAiService : IAiService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClaudeAiService> _logger;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _defaultMaxTokens;

        public ClaudeAiService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ClaudeAiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = configuration["AiSettings:ApiKey"]
                ?? throw new InvalidOperationException("AiSettings:ApiKey is not configured.");
            _model = configuration["AiSettings:Model"] ?? "claude-sonnet-4-20250514";
            _defaultMaxTokens = int.TryParse(configuration["AiSettings:MaxTokens"], out var mt) ? mt : 2048;
        }

        public async Task<string> ChatAsync(string systemPrompt, List<ChatMessage> messages, int maxTokens = 1024)
        {
            var effectiveMaxTokens = maxTokens > 0 ? maxTokens : _defaultMaxTokens;
            return await SendRequestAsync(systemPrompt, messages, effectiveMaxTokens);
        }

        public async Task<string> GenerateAsync(string systemPrompt, string userPrompt, int maxTokens = 2048)
        {
            var effectiveMaxTokens = maxTokens > 0 ? maxTokens : _defaultMaxTokens;
            var messages = new List<ChatMessage>
            {
                new() { Role = "user", Content = userPrompt }
            };
            return await SendRequestAsync(systemPrompt, messages, effectiveMaxTokens);
        }

        private async Task<string> SendRequestAsync(string systemPrompt, List<ChatMessage> messages, int maxTokens)
        {
            var client = _httpClientFactory.CreateClient("Anthropic");

            var requestBody = new
            {
                model = _model,
                max_tokens = maxTokens,
                system = systemPrompt,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToList()
            };

            var json = JsonSerializer.Serialize(requestBody, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            httpContent.Headers.Add("x-api-key", _apiKey);

            try
            {
                var response = await client.PostAsync("v1/messages", httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Claude API error {StatusCode}: {Body}",
                        (int)response.StatusCode,
                        responseBody);
                    throw new HttpRequestException(
                        $"Claude API returned {(int)response.StatusCode}: {responseBody}");
                }

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (!root.TryGetProperty("content", out var contentArray)
                    || contentArray.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Claude API returned empty content");
                    return string.Empty;
                }

                foreach (var block in contentArray.EnumerateArray())
                {
                    if (block.TryGetProperty("type", out var typeProp)
                        && typeProp.GetString() == "text"
                        && block.TryGetProperty("text", out var textProp))
                    {
                        return textProp.GetString() ?? string.Empty;
                    }
                }

                return string.Empty;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling Claude API");
                throw;
            }
        }
    }
}

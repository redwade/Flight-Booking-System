using AI.Core.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace AI.Infrastructure.Services;

public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaAIService> _logger;
    private readonly string _modelName = "gemma2:2b"; // Using Gemma 2 2B model (Gemma 3 not yet released in Ollama)
    private readonly Dictionary<string, List<Message>> _sessionHistory = new();

    public OllamaAIService(HttpClient httpClient, ILogger<OllamaAIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GenerateChatResponseAsync(string userMessage, string? sessionId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            sessionId ??= Guid.NewGuid().ToString();

            // Get or create session history
            if (!_sessionHistory.ContainsKey(sessionId))
            {
                _sessionHistory[sessionId] = new List<Message>
                {
                    new Message 
                    { 
                        Role = "system", 
                        Content = "You are a helpful flight booking assistant. Help users with flight searches, bookings, and travel-related questions. Be concise and friendly." 
                    }
                };
            }

            // Add user message to history
            _sessionHistory[sessionId].Add(new Message { Role = "user", Content = userMessage });

            var request = new OllamaRequest
            {
                Model = _modelName,
                Messages = _sessionHistory[sessionId],
                Stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken);
            var assistantMessage = result?.Message?.Content ?? "I apologize, but I couldn't generate a response.";

            // Add assistant response to history
            _sessionHistory[sessionId].Add(new Message { Role = "assistant", Content = assistantMessage });

            // Keep only last 10 messages to avoid context overflow
            if (_sessionHistory[sessionId].Count > 11) // 1 system + 10 messages
            {
                _sessionHistory[sessionId] = new List<Message> 
                { 
                    _sessionHistory[sessionId][0] // Keep system message
                }
                .Concat(_sessionHistory[sessionId].Skip(_sessionHistory[sessionId].Count - 10))
                .ToList();
            }

            return assistantMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating chat response");
            return "I'm having trouble connecting to the AI service. Please try again later.";
        }
    }

    public async Task<string> GenerateFlightRecommendationAsync(string userPreferences, CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = $@"Based on the following user preferences, provide flight recommendations:

{userPreferences}

Please provide 3-5 specific recommendations with reasons. Be concise and focus on matching the user's preferences.";

            var request = new OllamaRequest
            {
                Model = _modelName,
                Messages = new List<Message>
                {
                    new Message { Role = "system", Content = "You are a flight recommendation expert. Analyze user preferences and provide personalized flight recommendations." },
                    new Message { Role = "user", Content = prompt }
                },
                Stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken);
            return result?.Message?.Content ?? "Unable to generate recommendations.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating flight recommendations");
            return "Unable to generate recommendations at this time.";
        }
    }

    public async Task<string> AnalyzeBookingPatternAsync(string bookingData, CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = $@"Analyze the following booking pattern data and provide insights:

{bookingData}

Provide insights about:
1. Booking trends
2. Popular destinations
3. Peak booking times
4. Recommendations for optimization";

            var request = new OllamaRequest
            {
                Model = _modelName,
                Messages = new List<Message>
                {
                    new Message { Role = "system", Content = "You are a data analyst specializing in travel and booking patterns." },
                    new Message { Role = "user", Content = prompt }
                },
                Stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken);
            return result?.Message?.Content ?? "Unable to analyze booking patterns.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing booking patterns");
            return "Unable to analyze booking patterns at this time.";
        }
    }

    public async Task<bool> IsModelAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class OllamaRequest
    {
        public string Model { get; set; } = string.Empty;
        public List<Message> Messages { get; set; } = new();
        public bool Stream { get; set; }
    }

    private class Message
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    private class OllamaResponse
    {
        public Message? Message { get; set; }
        public bool Done { get; set; }
    }
}

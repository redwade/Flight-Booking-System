namespace AI.Core.Services;

public interface IAIService
{
    Task<string> GenerateChatResponseAsync(string userMessage, string? sessionId = null, CancellationToken cancellationToken = default);
    Task<string> GenerateFlightRecommendationAsync(string userPreferences, CancellationToken cancellationToken = default);
    Task<string> AnalyzeBookingPatternAsync(string bookingData, CancellationToken cancellationToken = default);
    Task<bool> IsModelAvailableAsync(CancellationToken cancellationToken = default);
}

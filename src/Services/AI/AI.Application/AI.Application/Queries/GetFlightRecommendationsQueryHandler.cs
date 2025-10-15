using AI.Core.Services;
using MediatR;
using System.Text;

namespace AI.Application.Queries;

public class GetFlightRecommendationsQueryHandler : IRequestHandler<GetFlightRecommendationsQuery, FlightRecommendationsResponse>
{
    private readonly IAIService _aiService;

    public GetFlightRecommendationsQueryHandler(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task<FlightRecommendationsResponse> Handle(GetFlightRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var preferences = new StringBuilder();
        preferences.AppendLine($"Origin: {request.Origin}");
        preferences.AppendLine($"Destination: {request.Destination}");
        
        if (request.DepartureDate.HasValue)
            preferences.AppendLine($"Departure Date: {request.DepartureDate.Value:yyyy-MM-dd}");
        
        if (!string.IsNullOrEmpty(request.PreferredClass))
            preferences.AppendLine($"Preferred Class: {request.PreferredClass}");
        
        if (request.MaxBudget.HasValue)
            preferences.AppendLine($"Maximum Budget: ${request.MaxBudget.Value}");

        var recommendations = await _aiService.GenerateFlightRecommendationAsync(preferences.ToString(), cancellationToken);

        return new FlightRecommendationsResponse(recommendations, DateTime.UtcNow);
    }
}

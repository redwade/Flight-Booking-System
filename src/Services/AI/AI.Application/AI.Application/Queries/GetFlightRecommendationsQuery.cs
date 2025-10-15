using MediatR;

namespace AI.Application.Queries;

public record GetFlightRecommendationsQuery(
    string UserId,
    string Origin,
    string Destination,
    DateTime? DepartureDate = null,
    string? PreferredClass = null,
    decimal? MaxBudget = null
) : IRequest<FlightRecommendationsResponse>;

public record FlightRecommendationsResponse(
    string Recommendations,
    DateTime GeneratedAt
);

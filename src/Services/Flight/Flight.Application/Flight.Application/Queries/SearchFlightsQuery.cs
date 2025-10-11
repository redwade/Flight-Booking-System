using MediatR;

namespace Flight.Application.Queries;

public record SearchFlightsQuery(
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureDate) : IRequest<IEnumerable<SearchFlightsQueryResponse>>;

public record SearchFlightsQueryResponse
{
    public Guid Id { get; init; }
    public string FlightNumber { get; init; } = string.Empty;
    public string Airline { get; init; } = string.Empty;
    public string DepartureAirport { get; init; } = string.Empty;
    public string ArrivalAirport { get; init; } = string.Empty;
    public DateTime DepartureTime { get; init; }
    public DateTime ArrivalTime { get; init; }
    public int AvailableSeats { get; init; }
    public decimal PricePerSeat { get; init; }
    public string Status { get; init; } = string.Empty;
}

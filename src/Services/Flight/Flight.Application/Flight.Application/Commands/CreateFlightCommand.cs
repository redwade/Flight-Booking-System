using MediatR;

namespace Flight.Application.Commands;

public record CreateFlightCommand : IRequest<CreateFlightCommandResponse>
{
    public string FlightNumber { get; init; } = string.Empty;
    public string Airline { get; init; } = string.Empty;
    public string DepartureAirport { get; init; } = string.Empty;
    public string ArrivalAirport { get; init; } = string.Empty;
    public DateTime DepartureTime { get; init; }
    public DateTime ArrivalTime { get; init; }
    public int TotalSeats { get; init; }
    public decimal PricePerSeat { get; init; }
    public string? AircraftType { get; init; }
}

public record CreateFlightCommandResponse
{
    public Guid FlightId { get; init; }
    public string FlightNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

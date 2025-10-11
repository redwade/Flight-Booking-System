namespace BuildingBlocks.MassTransit.Messages;

public record FlightSeatsUpdatedEvent
{
    public Guid FlightId { get; init; }
    public int AvailableSeats { get; init; }
    public DateTime UpdatedAt { get; init; }
}

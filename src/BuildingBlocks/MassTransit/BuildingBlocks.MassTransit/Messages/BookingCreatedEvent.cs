namespace BuildingBlocks.MassTransit.Messages;

public record BookingCreatedEvent
{
    public Guid BookingId { get; init; }
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime BookingDate { get; init; }
}

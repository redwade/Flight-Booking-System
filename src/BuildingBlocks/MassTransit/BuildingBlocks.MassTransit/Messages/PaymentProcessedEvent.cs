namespace BuildingBlocks.MassTransit.Messages;

public record PaymentProcessedEvent
{
    public Guid PaymentId { get; init; }
    public Guid BookingId { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public DateTime PaymentDate { get; init; }
}

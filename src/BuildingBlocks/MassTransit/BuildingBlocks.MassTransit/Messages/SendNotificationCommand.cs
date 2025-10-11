namespace BuildingBlocks.MassTransit.Messages;

public record SendNotificationCommand
{
    public Guid UserId { get; init; }
    public Guid? BookingId { get; init; }
    public string NotificationType { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
}

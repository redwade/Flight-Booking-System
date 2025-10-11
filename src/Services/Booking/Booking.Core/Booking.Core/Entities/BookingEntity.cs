namespace Booking.Core.Entities;

public class BookingEntity
{
    public Guid Id { get; set; }
    public Guid FlightId { get; set; }
    public Guid UserId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerEmail { get; set; } = string.Empty;
    public string PassengerPhone { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public string? BookingReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    PaymentPending = 3,
    PaymentCompleted = 4,
    PaymentFailed = 5
}

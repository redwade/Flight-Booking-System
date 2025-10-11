using MediatR;

namespace Booking.Application.Commands;

public record CreateBookingCommand : IRequest<CreateBookingCommandResponse>
{
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public string PassengerPhone { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
}

public record CreateBookingCommandResponse
{
    public Guid BookingId { get; init; }
    public string BookingReference { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

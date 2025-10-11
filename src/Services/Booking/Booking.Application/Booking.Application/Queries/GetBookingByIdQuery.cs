using MediatR;

namespace Booking.Application.Queries;

public record GetBookingByIdQuery(Guid BookingId) : IRequest<GetBookingByIdQueryResponse?>;

public record GetBookingByIdQueryResponse
{
    public Guid Id { get; init; }
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public string PassengerPhone { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime BookingDate { get; init; }
    public string? BookingReference { get; init; }
}

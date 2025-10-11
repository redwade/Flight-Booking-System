using MediatR;

namespace Payment.Application.Queries;

public record GetPaymentByBookingIdQuery(Guid BookingId) : IRequest<IEnumerable<GetPaymentByBookingIdQueryResponse>>;

public record GetPaymentByBookingIdQueryResponse
{
    public Guid Id { get; init; }
    public Guid BookingId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public DateTime PaymentDate { get; init; }
}

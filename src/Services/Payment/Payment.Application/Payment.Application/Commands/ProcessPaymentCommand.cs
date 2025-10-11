using MediatR;

namespace Payment.Application.Commands;

public record ProcessPaymentCommand : IRequest<ProcessPaymentCommandResponse>
{
    public Guid BookingId { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string PaymentMethod { get; init; } = string.Empty;
}

public record ProcessPaymentCommandResponse
{
    public Guid PaymentId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
}

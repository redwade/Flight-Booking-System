using MediatR;
using Payment.Core.Repositories;

namespace Payment.Application.Queries;

public class GetPaymentByBookingIdQueryHandler : IRequestHandler<GetPaymentByBookingIdQuery, IEnumerable<GetPaymentByBookingIdQueryResponse>>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByBookingIdQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GetPaymentByBookingIdQueryResponse>> Handle(GetPaymentByBookingIdQuery request, CancellationToken cancellationToken)
    {
        var payments = await _repository.GetByBookingIdAsync(request.BookingId, cancellationToken);

        return payments.Select(p => new GetPaymentByBookingIdQueryResponse
        {
            Id = p.Id,
            BookingId = p.BookingId,
            Amount = p.Amount,
            Currency = p.Currency,
            PaymentMethod = p.PaymentMethod.ToString(),
            Status = p.Status.ToString(),
            TransactionId = p.TransactionId,
            PaymentDate = p.PaymentDate
        });
    }
}

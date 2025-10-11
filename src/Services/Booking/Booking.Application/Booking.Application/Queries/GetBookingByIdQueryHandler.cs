using Booking.Core.Repositories;
using MediatR;

namespace Booking.Application.Queries;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, GetBookingByIdQueryResponse?>
{
    private readonly IBookingRepository _repository;

    public GetBookingByIdQueryHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetBookingByIdQueryResponse?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken);
        
        if (booking == null)
            return null;

        return new GetBookingByIdQueryResponse
        {
            Id = booking.Id,
            FlightId = booking.FlightId,
            UserId = booking.UserId,
            PassengerName = booking.PassengerName,
            PassengerEmail = booking.PassengerEmail,
            PassengerPhone = booking.PassengerPhone,
            NumberOfSeats = booking.NumberOfSeats,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status.ToString(),
            BookingDate = booking.BookingDate,
            BookingReference = booking.BookingReference
        };
    }
}

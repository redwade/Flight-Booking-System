using Booking.Core.Entities;
using Booking.Core.Repositories;
using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using MediatR;

namespace Booking.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingCommandResponse>
{
    private readonly IBookingRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateBookingCommandHandler(IBookingRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateBookingCommandResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new BookingEntity
        {
            FlightId = request.FlightId,
            UserId = request.UserId,
            PassengerName = request.PassengerName,
            PassengerEmail = request.PassengerEmail,
            PassengerPhone = request.PassengerPhone,
            NumberOfSeats = request.NumberOfSeats,
            TotalAmount = request.TotalAmount,
            Status = BookingStatus.Pending,
            BookingDate = DateTime.UtcNow,
            BookingReference = GenerateBookingReference()
        };

        var createdBooking = await _repository.CreateAsync(booking, cancellationToken);

        // Publish booking created event
        await _publishEndpoint.Publish(new BookingCreatedEvent
        {
            BookingId = createdBooking.Id,
            FlightId = createdBooking.FlightId,
            UserId = createdBooking.UserId,
            PassengerName = createdBooking.PassengerName,
            PassengerEmail = createdBooking.PassengerEmail,
            NumberOfSeats = createdBooking.NumberOfSeats,
            TotalAmount = createdBooking.TotalAmount,
            BookingDate = createdBooking.BookingDate
        }, cancellationToken);

        return new CreateBookingCommandResponse
        {
            BookingId = createdBooking.Id,
            BookingReference = createdBooking.BookingReference!,
            Status = createdBooking.Status.ToString()
        };
    }

    private static string GenerateBookingReference()
    {
        return $"BK{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}

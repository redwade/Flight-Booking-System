using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using Notification.Core.Entities;
using Notification.Core.Repositories;

namespace Notification.Application.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly INotificationRepository _repository;

    public BookingCreatedConsumer(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var message = context.Message;

        var notification = new NotificationEntity
        {
            UserId = message.UserId,
            BookingId = message.BookingId,
            Type = NotificationType.Email,
            Subject = "Booking Confirmation",
            Message = $"Your booking for flight {message.FlightId} has been created successfully. Booking Reference: {message.BookingId}",
            Recipient = message.PassengerEmail,
            Status = NotificationStatus.Pending
        };

        await _repository.CreateAsync(notification);

        // Simulate sending notification
        await Task.Delay(500);
        
        notification.Status = NotificationStatus.Sent;
        notification.SentAt = DateTime.UtcNow;
        await _repository.UpdateAsync(notification);
    }
}

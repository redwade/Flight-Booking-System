using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using Notification.Core.Entities;
using Notification.Core.Repositories;

namespace Notification.Application.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly INotificationRepository _repository;

    public PaymentProcessedConsumer(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;

        var notification = new NotificationEntity
        {
            UserId = message.UserId,
            BookingId = message.BookingId,
            Type = NotificationType.Email,
            Subject = "Payment Confirmation",
            Message = $"Your payment of {message.Amount} has been {message.PaymentStatus}. Transaction ID: {message.TransactionId}",
            Recipient = "user@example.com", // Should be fetched from user service
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

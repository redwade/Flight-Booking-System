using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using Notification.Core.Entities;
using Notification.Core.Repositories;

namespace Notification.Application.Consumers;

public class SendNotificationConsumer : IConsumer<SendNotificationCommand>
{
    private readonly INotificationRepository _repository;

    public SendNotificationConsumer(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<SendNotificationCommand> context)
    {
        var message = context.Message;

        var notification = new NotificationEntity
        {
            UserId = message.UserId,
            BookingId = message.BookingId,
            Type = Enum.Parse<NotificationType>(message.NotificationType, true),
            Subject = message.Subject,
            Message = message.Message,
            Recipient = message.Recipient,
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

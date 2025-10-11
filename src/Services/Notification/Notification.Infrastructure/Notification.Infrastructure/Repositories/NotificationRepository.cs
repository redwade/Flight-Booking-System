using Notification.Core.Entities;
using Notification.Core.Repositories;
using System.Collections.Concurrent;

namespace Notification.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ConcurrentDictionary<Guid, NotificationEntity> _notifications = new();

    public Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _notifications.TryGetValue(id, out var notification);
        return Task.FromResult(notification);
    }

    public Task<IEnumerable<NotificationEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<NotificationEntity>>(_notifications.Values.ToList());
    }

    public Task<IEnumerable<NotificationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = _notifications.Values.Where(n => n.UserId == userId).ToList();
        return Task.FromResult<IEnumerable<NotificationEntity>>(notifications);
    }

    public Task<IEnumerable<NotificationEntity>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        var notifications = _notifications.Values.Where(n => n.BookingId == bookingId).ToList();
        return Task.FromResult<IEnumerable<NotificationEntity>>(notifications);
    }

    public Task<IEnumerable<NotificationEntity>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = _notifications.Values
            .Where(n => n.Status == NotificationStatus.Pending)
            .ToList();
        return Task.FromResult<IEnumerable<NotificationEntity>>(notifications);
    }

    public Task<NotificationEntity> CreateAsync(NotificationEntity notification, CancellationToken cancellationToken = default)
    {
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.UtcNow;
        _notifications.TryAdd(notification.Id, notification);
        return Task.FromResult(notification);
    }

    public Task<NotificationEntity> UpdateAsync(NotificationEntity notification, CancellationToken cancellationToken = default)
    {
        notification.UpdatedAt = DateTime.UtcNow;
        _notifications[notification.Id] = notification;
        return Task.FromResult(notification);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_notifications.TryRemove(id, out _));
    }
}

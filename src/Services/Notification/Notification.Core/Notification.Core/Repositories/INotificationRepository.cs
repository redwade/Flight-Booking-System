using Notification.Core.Entities;

namespace Notification.Core.Repositories;

public interface INotificationRepository
{
    Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
    Task<NotificationEntity> CreateAsync(NotificationEntity notification, CancellationToken cancellationToken = default);
    Task<NotificationEntity> UpdateAsync(NotificationEntity notification, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

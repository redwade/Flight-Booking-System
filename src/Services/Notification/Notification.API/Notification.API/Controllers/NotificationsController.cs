using Microsoft.AspNetCore.Mvc;
using Notification.Core.Repositories;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationRepository repository, ILogger<NotificationsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetNotificationsByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await _repository.GetByUserIdAsync(userId, cancellationToken);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving notifications");
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetNotificationsByBookingId(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await _repository.GetByBookingIdAsync(bookingId, cancellationToken);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for booking {BookingId}", bookingId);
            return StatusCode(500, "An error occurred while retrieving notifications");
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingNotifications(CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await _repository.GetPendingNotificationsAsync(cancellationToken);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending notifications");
            return StatusCode(500, "An error occurred while retrieving pending notifications");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Notification API", timestamp = DateTime.UtcNow });
    }
}

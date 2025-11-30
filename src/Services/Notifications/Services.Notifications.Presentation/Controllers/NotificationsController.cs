using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.SystemAdmins;
using Services.Notifications.Domain.Entities;

namespace Services.Notifications.Presentation.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
#pragma warning disable CA1515 // Consider making public types internal
public class NotificationsController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{notificationId}")]
    public async Task<ActionResult<Notification>> GetNotificationById(string notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);
        
        if (notification == null)
        {
            return NotFound($"Notification with ID {notificationId} not found.");
        }

        return Ok(notification);
    }

    /// <summary>
    /// Get all notifications by admin ID
    /// </summary>
    [HttpGet("admin/{adminId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByAdminId(string adminId)
    {
        IEnumerable<Notification> notifications = await _notificationService.GetByAdminIdAsync(adminId);
        return Ok(notifications);
    }

    /// <summary>
    /// Get unread notifications by admin ID
    /// </summary>
    [HttpGet("admin/{adminId}/unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUnreadNotificationsByAdminId(string adminId)
    {
        IEnumerable<Notification> notifications = await _notificationService.GetUnreadByAdminIdAsync(adminId);
        return Ok(notifications);
    }

    /// <summary>
    /// Count unread notifications by admin ID
    /// </summary>
    [HttpGet("admin/{adminId}/unread/count")]
    public async Task<ActionResult<CountResponseDto>> CountUnreadNotificationsByAdminId(string adminId)
    {
        long count = await _notificationService.CountUnreadByAdminIdAsync(adminId);
        return Ok(new CountResponseDto { Count = count });
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
    {
        bool result = await _notificationService.MarkAsReadAsync(notificationId);
        
        if (!result)
        {
            return NotFound($"Notification with ID {notificationId} not found.");
        }

        return Ok();
    }
}

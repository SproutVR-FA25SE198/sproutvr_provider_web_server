using Services.Notifications.Domain.Entities;

namespace Services.Notifications.Application.Abstractions.Services;

public interface INotificationService
{
    Task<Notification?> GetByIdAsync(string id);
    Task<IEnumerable<Notification>> GetByAdminIdAsync(string adminId);
    Task<IEnumerable<Notification>> GetUnreadByAdminIdAsync(string adminId);
    Task<long> CountUnreadByAdminIdAsync(string adminId);
    Task<bool> MarkAsReadAsync(string id);
    Task<Notification> CreateAsync(Notification notification);
}


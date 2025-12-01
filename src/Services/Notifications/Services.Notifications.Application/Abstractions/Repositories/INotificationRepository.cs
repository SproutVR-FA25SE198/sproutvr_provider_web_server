using Services.Notifications.Domain.Entities;

namespace Services.Notifications.Application.Abstractions.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string id);
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<IEnumerable<Notification>> GetByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<Notification>> GetByAdminIdAsync(string adminId);
    Task<IEnumerable<Notification>> GetUnreadByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<Notification>> GetUnreadByAdminIdAsync(string adminId);
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification> UpdateAsync(Notification notification);
    Task<bool> DeleteAsync(string id);
    Task<bool> MarkAsReadAsync(string id);
    Task<long> CountUnreadByOrganizationIdAsync(string organizationId);
    Task<long> CountUnreadByAdminIdAsync(string adminId);
}


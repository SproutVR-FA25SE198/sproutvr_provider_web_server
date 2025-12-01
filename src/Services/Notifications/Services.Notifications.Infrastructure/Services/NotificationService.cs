using Services.Notifications.Application.Abstractions.Repositories;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Domain.Entities;

namespace Services.Notifications.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _notificationRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByAdminIdAsync(string adminId)
    {
        return await _notificationRepository.GetByAdminIdAsync(adminId);
    }

    public async Task<IEnumerable<Notification>> GetUnreadByAdminIdAsync(string adminId)
    {
        return await _notificationRepository.GetUnreadByAdminIdAsync(adminId);
    }

    public async Task<long> CountUnreadByAdminIdAsync(string adminId)
    {
        return await _notificationRepository.CountUnreadByAdminIdAsync(adminId);
    }

    public async Task<bool> MarkAsReadAsync(string id)
    {
        return await _notificationRepository.MarkAsReadAsync(id);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        return await _notificationRepository.CreateAsync(notification);
    }
}


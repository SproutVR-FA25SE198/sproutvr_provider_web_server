using MongoDB.Driver;
using Services.Notifications.Application.Abstractions.Repositories;
using Services.Notifications.Domain.Entities;
using Services.Notifications.Infrastructure.Data;

namespace Services.Notifications.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notifications;

    public NotificationRepository(MongoDbContext context)
    {
        _notifications = context.Notifications;
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _notifications.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetByOrganizationIdAsync(string organizationId)
    {
        return await _notifications.Find(n => n.OrganizationId == organizationId).ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetByAdminIdAsync(string adminId)
    {
        return await _notifications.Find(n => n.AdminId == adminId).ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByOrganizationIdAsync(string organizationId)
    {
        return await _notifications.Find(n => n.OrganizationId == organizationId && !n.IsRead).ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByAdminIdAsync(string adminId)
    {
        return await _notifications.Find(n => n.AdminId == adminId && !n.IsRead).ToListAsync();
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        notification.CreatedAt = DateTime.UtcNow;
        await _notifications.InsertOneAsync(notification);
        return notification;
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        await _notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification);
        return notification;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        DeleteResult result = await _notifications.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> MarkAsReadAsync(string id)
    {
        UpdateDefinition<Notification> update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow);

        UpdateResult result = await _notifications.UpdateOneAsync(n => n.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<long> CountUnreadByOrganizationIdAsync(string organizationId)
    {
        return await _notifications.CountDocumentsAsync(n => n.OrganizationId == organizationId && !n.IsRead);
    }

    public async Task<long> CountUnreadByAdminIdAsync(string adminId)
    {
        return await _notifications.CountDocumentsAsync(n => n.AdminId == adminId && !n.IsRead);
    }
}


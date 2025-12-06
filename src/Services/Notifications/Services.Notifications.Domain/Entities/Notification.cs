using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Services.Notifications.Domain.Entities;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("organizationId")]
    [BsonRepresentation(BsonType.String)]
    public string? OrganizationId { get; set; }

    [BsonElement("adminId")]
    [BsonRepresentation(BsonType.String)]
    public string? AdminId { get; set; }

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("readAt")]
    public DateTime? ReadAt { get; set; }
}

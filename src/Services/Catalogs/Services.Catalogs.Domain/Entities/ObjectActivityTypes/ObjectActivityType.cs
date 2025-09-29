using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.ActivityTypes;
using Services.Catalogs.Domain.Entities.MapObjects;

namespace Services.Catalogs.Domain.Entities.ObjectActivityTypes;
public class ObjectActivityType : BaseEntity
{
    public Guid MapObjectId { get; set; }
    public Guid ActivityTypeId { get; set; }

    // navigation properties
    public MapObject MapObject { get; set; }
    public ActivityType ActivityType { get; set; }
}

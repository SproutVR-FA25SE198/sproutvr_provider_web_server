using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Domain.Entities.ObjectLocations;
public class ObjectLocation : BaseEntity
{
    public Guid ObjectId { get; set; }
    public Guid LocationId { get; set; }
    public override bool UseIdKey => false;
    // navigation property
    public TaskLocation TaskLocation { get; set; }
    public MapObject MapObject { get; set; }
}

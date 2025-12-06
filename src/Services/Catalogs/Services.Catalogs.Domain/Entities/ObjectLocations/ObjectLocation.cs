using System.Text.Json.Serialization;
using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Domain.Entities.ObjectLocations;
public class ObjectLocation : BaseEntity
{
    public Guid ObjectId { get; set; }
    public Guid TaskLocationId { get; set; }
    public override bool UseIdKey => false;
    // navigation property
    [JsonIgnore]
    public TaskLocation TaskLocation { get; set; }
    [JsonIgnore]
    public MapObject MapObject { get; set; }
}

using System.Text.Json.Serialization;
using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Domain.Entities.TaskLocations;
public class TaskLocation : BaseEntity
{
    public Guid MapId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string LocationCode { get; set; }

    // navigation property
    [JsonIgnore]
    public Map Map { get; set; }
    [JsonIgnore]
    public List<ObjectLocation> ObjectLocations { get; set; } = [];
}

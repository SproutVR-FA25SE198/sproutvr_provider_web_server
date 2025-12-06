using System.Text.Json.Serialization;
using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;

namespace Services.Catalogs.Domain.Entities.MapObjects;
public class MapObject : BaseEntity
{
    public Guid MapId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string ObjectCode { get; set; }

    // navigation property
    [JsonIgnore]
    public Map Map { get; set; }
    [JsonIgnore]
    public List<ObjectActivityType> ObjectActivityTypes { get; set; } = [];
}

using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;

namespace Services.Catalogs.Domain.Entities.MapObjects;
public class MapObject : BaseEntity
{
    public Guid MapId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }

    // navigation property
    public Map Map { get; set; }
    public List<ObjectActivityType> ObjectActivityTypes { get; set; } = [];
}

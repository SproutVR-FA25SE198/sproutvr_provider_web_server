using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Domain.Entities.Maps;
public sealed class Map : BaseEntity
{
    public Guid SubjectId { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public MapStatus Status { get; set; }

    // navigation property
    public Subject Subject { get; set; }
    public List<TaskLocation> TaskLocations { get; set; } = [];
    public List<MapObject> MapObjects { get; set; } = [];
}


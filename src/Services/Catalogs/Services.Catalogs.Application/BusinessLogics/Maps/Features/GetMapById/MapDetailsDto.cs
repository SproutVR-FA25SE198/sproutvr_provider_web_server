using Services.Catalogs.Application.BusinessLogics.MapObjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.TaskLocations.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapById;

public class MapDetailsDto
{
    public Guid Id { get; set; }
    public SubjectDto Subject { get; set; } = new();
    public decimal Price { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string MapCode { get; set; } = string.Empty;
    public List<MapObjectDto> MapObjects { get; set; } = [];
    public List<TaskLocationDto> TaskLocations { get; set; } = [];
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

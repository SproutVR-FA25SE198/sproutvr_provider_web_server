using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
public class MapDto
{
    public Guid Id { get; set; }
    public SubjectDto Subject { get; set; } = new();
    public decimal Price { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string PreviewUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string MapCode { get; set; } = string.Empty;
}

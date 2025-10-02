using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;

public class CreateMapDto
{
    public Guid SubjectId { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string MapCode { get; set; } = string.Empty;
}

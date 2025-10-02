using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.UpdateMap;
public class UpdateMapDto
{
    public Guid? SubjectId { get; set; }
    public decimal? Price { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Status { get; set; }
    public string? MapCode { get; set; }
}

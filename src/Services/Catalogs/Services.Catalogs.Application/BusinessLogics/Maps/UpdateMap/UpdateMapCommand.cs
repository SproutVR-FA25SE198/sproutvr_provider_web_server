using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.UpdateMap;
public sealed class UpdateMapCommand : IRequest<MapDto>
{
    public Guid Id { get; set; }
    public Guid? SubjectId { get; set; }
    public decimal? Price { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public MapStatus? Status { get; set; }
    public string? MapCode { get; set; }
}

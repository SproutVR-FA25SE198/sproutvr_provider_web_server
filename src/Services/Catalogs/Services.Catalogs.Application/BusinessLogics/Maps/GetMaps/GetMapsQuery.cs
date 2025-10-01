using Common.Application.Helpers;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Maps.GetMaps;
public class GetMapsQuery(MapParams mapParams) : IRequest<PaginatedResult<MapDto>>
{
    public MapParams SpecParams { get; set; } = mapParams;
}

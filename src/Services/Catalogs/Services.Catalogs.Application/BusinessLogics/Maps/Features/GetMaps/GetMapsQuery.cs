using Common.Application.Helpers;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Specifications;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
public class GetMapsQuery(MapParams mapParams) : IRequest<PaginatedResult<MapDto>>
{
    public MapParams SpecParams { get; set; } = mapParams;
}

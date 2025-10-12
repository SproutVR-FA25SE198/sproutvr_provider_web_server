using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapsByIds;
public class GetMapsByIdsQuery(List<string> ids) : IRequest<List<MapDto>>
{
    public List<string> Ids { get; } = ids;
}

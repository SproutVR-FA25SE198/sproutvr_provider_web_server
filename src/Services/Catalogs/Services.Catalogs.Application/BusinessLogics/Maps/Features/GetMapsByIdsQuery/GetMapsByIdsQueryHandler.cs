using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapsByIdsQuery;
public class GetMapsByIdsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMapsByIdsQuery, List<MapDto>>
{
    public async Task<List<MapDto>> Handle(GetMapsByIdsQuery request, CancellationToken cancellationToken)
    {
        var result = new List<MapDto>();
        foreach (string idRaw in request.Ids)
        {
            var id = Guid.Parse(idRaw);
            var spec = new MapSpecification(id, true);
            Map map = await unitOfWork.Repository<Map>().GetEntityWithSpec(spec);

            if (map == null)
            {
                throw new NotFoundException("Map not found!");
            }
            result.Add(MapMappings.ToDto(map));
        }
        return result;
    }
}

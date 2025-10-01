using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.GetMapById;

public sealed class GetMapByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMapByIdQuery, MapDto>
{
    public async Task<MapDto> Handle(GetMapByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new MapSpecification(request.Id);
        Map map = await unitOfWork.Repository<Map>().GetEntityWithSpec(spec);
        
        if (map == null)
        {
            throw new NotFoundException("Map not found!");
        }
        
        return map.ToDto();
    }
}

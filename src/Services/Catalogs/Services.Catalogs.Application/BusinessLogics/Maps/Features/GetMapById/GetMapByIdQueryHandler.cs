using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapById;

public sealed class GetMapByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMapByIdQuery, MapDetailsDto>
{
    public async Task<MapDetailsDto> Handle(GetMapByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new MapSpecification(request.Id, true);
        Map map = await unitOfWork.Repository<Map>().GetEntityWithSpec(spec);
        
        if (map == null)
        {
            throw new NotFoundException("Map not found!");
        }
        
        return map.ToDetailsDto();
    }
}

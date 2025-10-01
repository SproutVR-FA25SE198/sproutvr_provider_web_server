using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.UpdateMap;

public sealed class UpdateMapCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateMapCommand, MapDto>
{
    public async Task<MapDto> Handle(UpdateMapCommand request, CancellationToken cancellationToken)
    {
        Map map = await unitOfWork.Repository<Map>().GetByIdAsync(request.Id);
        
        if (map == null)
        {
            throw new NotFoundException("Map not found!");
        }

        map = MapMappings.ToEntity(request.Dto, map);

        unitOfWork.Repository<Map>().Update(map);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return map.ToDto();
    }
}

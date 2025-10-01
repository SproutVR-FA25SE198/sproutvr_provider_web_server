using Common.Application.Abstractions.Data;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.CreateMap;

public sealed class CreateMapCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateMapCommand, MapDto>
{
    public async Task<MapDto> Handle(CreateMapCommand request, CancellationToken cancellationToken)
    {
        Map map = MapMappings.ToEntity(request.Dto);

        unitOfWork.Repository<Map>().Add(map);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return map.ToDto();
    }
}

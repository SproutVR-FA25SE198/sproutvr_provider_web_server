using Common.Application.Abstractions.Data;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;

public sealed class CreateMapCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateMapCommand, MapDto>
{
    public async Task<MapDto> Handle(CreateMapCommand request, CancellationToken cancellationToken)
    {
        Map map = request.Dto.ToEntity();

        unitOfWork.Repository<Map>().Add(map);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return map.ToDto();
    }
}

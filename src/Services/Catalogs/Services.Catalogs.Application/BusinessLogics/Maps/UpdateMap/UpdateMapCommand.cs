using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.UpdateMap;
public sealed class UpdateMapCommand(UpdateMapDto updateMapDto) : IRequest<MapDto>
{
    public Guid Id { get; set; }
    public UpdateMapDto Dto { get; set; } = updateMapDto;
    
}

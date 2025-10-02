using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.UpdateMap;
public sealed class UpdateMapCommand(UpdateMapDto updateMapDto) : IRequest<MapDto>
{
    public Guid Id { get; set; }
    public UpdateMapDto Dto { get; set; } = updateMapDto;
    
}

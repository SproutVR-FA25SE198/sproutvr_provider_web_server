using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;
public sealed class CreateMapCommand(CreateMapDto dto) : IRequest<MapDto>
{
    public CreateMapDto Dto { get; set; } = dto;
}

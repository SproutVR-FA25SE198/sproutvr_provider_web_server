using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Maps.GetMapById;
public class GetMapByIdQuery(Guid id) : IRequest<MapDto>
{
    public Guid Id { get; set; } = id;
}

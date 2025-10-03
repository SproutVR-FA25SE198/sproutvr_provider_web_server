using MediatR;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapById;
public class GetMapByIdQuery(Guid id) : IRequest<MapDetailsDto>
{
    public Guid Id { get; set; } = id;
}

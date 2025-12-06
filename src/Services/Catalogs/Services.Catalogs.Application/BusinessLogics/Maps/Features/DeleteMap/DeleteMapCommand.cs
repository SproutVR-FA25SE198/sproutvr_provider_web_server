using MediatR;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.DeleteMap;

public sealed class DeleteMapCommand(Guid id) : IRequest
{
    public Guid Id { get; set; } = id;
}

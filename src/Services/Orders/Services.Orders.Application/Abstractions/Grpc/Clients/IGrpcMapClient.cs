using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

namespace Services.Orders.Application.Abstractions.Grpc.Clients;
public interface IGrpcMapClient
{
    Task<IReadOnlyList<MapDto>> GetMapsByIdsAsync(List<string> ids);
}

using OrdersService;
namespace Services.Payments.Application.Abstractions.Grpc.Client;
public interface IGrpcOrderClient
{
    Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequest request);
}

using OrdersService;
namespace Services.Payments.Application.Abstractions.Grpc.Client;
public interface IGrpcOrderClient
{
    Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(UpdateOrderStatusRequest request);
}

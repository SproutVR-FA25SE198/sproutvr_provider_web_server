using PaymentsService;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

namespace Services.Orders.Application.Abstractions.Grpc.Clients;
public interface IGrpcPaymentClient
{
    Task<OrderResponseDto> CreatePaymentAsync(CreatePaymentRequest request);
}

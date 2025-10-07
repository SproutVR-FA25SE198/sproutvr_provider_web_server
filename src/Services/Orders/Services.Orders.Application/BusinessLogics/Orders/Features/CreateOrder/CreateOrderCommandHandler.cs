using Common.Application.Abstractions.Data;
using MediatR;
using PaymentsService;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.OrderItems.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.Helpers;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork, 
    IGrpcMapClient grpcMapClient,
    IGrpcPaymentClient grpcPaymentClient) : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
    // Validate order information
        Order order = OrderMappings.ToEntity(request.CreateOrderDto);
        decimal totalMoneyAmount = 0;
        
        var mapIds = request.CreateOrderDto.Basket.BasketItems.Select(i => i.MapId).ToList();

        // Get map details from Catalogs service
        IReadOnlyList<MapDto> maps = await grpcMapClient.GetMapsByIdsAsync(mapIds);
        foreach (MapDto item in maps)
        {
            totalMoneyAmount += item.Price;
            order.OrderItems.Add(OrderItemMappings.ToEntity(item));
        }
        order.TotalMoneyAmount = totalMoneyAmount;

        // Generate unique order code
        int orderCode = OrderUtils.GenerateOrderCode();
        order.OrderCode = orderCode;

    // Integrate with Payment service
        var paymentRequest = new CreatePaymentRequest
        {
            OrderId = order.Id.ToString(),
            TotalMoneyAmount = (int)order.TotalMoneyAmount,
            PaymentMethod = request.CreateOrderDto.PaymentMethod,
            OrderCode = orderCode,
        };
        OrderResponseDto paymentResponse = await grpcPaymentClient.CreatePaymentAsync(paymentRequest);

    // Save new order to database with status pending
        unitOfWork.Repository<Order>().Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return paymentResponse;
    }

}

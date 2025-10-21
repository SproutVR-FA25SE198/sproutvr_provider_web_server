using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
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
        CreateOrderDto dto = request.CreateOrderDto;
        Order order = OrderMappings.ToEntity(dto);
        order.Status = OrderStatus.Payment_Pending;
        decimal totalMoneyAmount = 0;
        
        var mapIds = dto.Basket.BasketItems.Select(i => i.MapId).ToList();

        // Get map details from Catalogs service
        IReadOnlyList<MapDto> maps = await grpcMapClient.GetMapsByIdsAsync(mapIds);
        foreach (MapDto item in maps)
        {
            totalMoneyAmount += item.Price;
            order.OrderItems.Add(OrderItemMappings.ToEntity(item));
        }
        order.TotalMoneyAmount = totalMoneyAmount;
        order.TotalItems = order.OrderItems.Count;

        // Generate unique order code
        long orderCode = OrderUtils.GenerateOrderCode();
        order.PayosOrderCode = orderCode;

    // Integrate with Payment service
        var paymentRequest = new CreatePaymentRequest
        {
            OrderId = order.Id.ToString(),
            TotalMoneyAmount = (int)order.TotalMoneyAmount,
            PaymentMethod = dto.PaymentMethod,
            OrderCode = orderCode,
        };
        OrderResponseDto paymentResponse = await grpcPaymentClient.CreatePaymentAsync(paymentRequest);

    // Save new order to database with status pending
        unitOfWork.Repository<Order>().Add(order);
        bool result = await unitOfWork.SaveChangesAsync(cancellationToken);

        paymentResponse.OrderId = order.Id;

        if (!result)
        {
            throw new OperationFailedException("Failed to create order");
        }

        return paymentResponse;
    }

}

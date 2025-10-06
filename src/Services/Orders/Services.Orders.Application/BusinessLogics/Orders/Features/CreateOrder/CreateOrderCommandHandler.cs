using Common.Application.Abstractions.Data;
using MediatR;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderCommandHandler(IUnitOfWork unitOfWork, IGrpcMapClient grpcMapClient) : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Order order = OrderMappings.ToEntity(request.CreateOrderDto);
        decimal totalMoneyAmount = 0;
        var mapIds = request.CreateOrderDto.Basket.BasketItems.Select(i => i.MapId).ToList();
        IReadOnlyList<MapDto> maps = await grpcMapClient.GetMapsByIdsAsync(mapIds);
        foreach (MapDto item in maps)
        {
            totalMoneyAmount += item.Price;
            order.OrderItems.Add(new OrderItem
            {
                MapId = item.MapId,
                MapName = item.MapName,
                MapCode = item.MapCode,
                Price = item.Price,
                ImageUrl = item.ImageUrl
            });
        }
        order.TotalMoneyAmount = totalMoneyAmount;
        unitOfWork.Repository<Order>().Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new OrderResponseDto
        {
            OrderId = order.Id,
            TotalMoneyAmount = order.TotalMoneyAmount
        };
    }
}

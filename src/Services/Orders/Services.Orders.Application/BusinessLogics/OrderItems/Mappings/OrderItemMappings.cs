using Services.Orders.Application.BusinessLogics.OrderItems.DTOs;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Application.BusinessLogics.OrderItems.Mappings;
public static class OrderItemMappings
{
    public static OrderItemDto ToDto (this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            OrderId = orderItem.OrderId,
            MapId = orderItem.MapId,
        };
    }
}

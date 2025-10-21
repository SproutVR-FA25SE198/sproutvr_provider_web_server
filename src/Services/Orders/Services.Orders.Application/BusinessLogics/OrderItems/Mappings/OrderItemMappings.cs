using Common.Application.Contracts.Orders;
using Services.Orders.Application.BusinessLogics.OrderItems.DTOs;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
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
            MapName = orderItem.MapName,
            MapCode = orderItem.MapCode,
            Price = orderItem.Price,
            ImageUrl = orderItem.ImageUrl,
            SubjectName = orderItem.SubjectName,
        };
    }

    public static MapDto ToMapDto(this OrderItem orderItem)
    {
        return new MapDto
        {
            MapId = orderItem.MapId,
            MapName = orderItem.MapName,
            MapCode = orderItem.MapCode,
            Price = orderItem.Price,
            ImageUrl = orderItem.ImageUrl,
            SubjectName = orderItem.SubjectName,
        };
    }

    public static OrderItem ToEntity (this MapDto orderItem)
    {
        return new OrderItem
        {
            MapId = orderItem.MapId,
            MapName = orderItem.MapName,
            MapCode = orderItem.MapCode,
            Price = orderItem.Price,
            ImageUrl = orderItem.ImageUrl,
            SubjectName = orderItem.SubjectName
        };
    }

    public static OrderItemMessage ToMessage(this OrderItem orderItem)
    {
        return new OrderItemMessage
        {
            MapId = orderItem.MapId,
            MapName = orderItem.MapName,
            MapCode = orderItem.MapCode,
            Price = orderItem.Price,
            ImageUrl = orderItem.ImageUrl,
            SubjectName = orderItem.SubjectName,
        };
    }
}

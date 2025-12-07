using Common.Application.Contracts.Orders;
using Services.Orders.Application.BusinessLogics.OrderItems.DTOs;
using Services.Orders.Application.BusinessLogics.OrderItems.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderById;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Mappings;
public static class OrderMappings
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrganizationId = order.OrganizationId,
            TotalItems = order.TotalItems,
            TotalMoneyAmount = order.TotalMoneyAmount,
            OrderCode = order.OrderCode,
            RepresentativeName = order.RepresentativeName,
            RepresentativePhone = order.RepresentativePhone,
            Status = order.Status.ToString(),
            CreatedAtUtc = order.CreatedAtUtc,
            UpdatedAtUtc = order.UpdatedAtUtc
        };
    }
    public static OrderDetailsDto ToDetailsDto(this Order order)
    {
        return new OrderDetailsDto
        {
            Id = order.Id,
            OrganizationId = order.OrganizationId,
            TotalItems = order.TotalItems,
            TotalMoneyAmount = order.TotalMoneyAmount,
            OrderCode = order.OrderCode,
            Status = order.Status.ToString(),
            RepresentativeName = order.RepresentativeName,
            RepresentativePhone = order.RepresentativePhone,
            CreatedAtUtc = order.CreatedAtUtc,
            UpdatedAtUtc = order.UpdatedAtUtc,
            OrderItems = order.OrderItems?.Select(oi => oi.ToDto()).ToList() ?? new List<OrderItemDto>(),
            ActivationKey = order.ActivationKey
        };
    }

    public static Order ToEntity(this CreateOrderDto orderDto)
    {
        return new Order
        {
            OrganizationId = orderDto.OrganizationId,
            TotalMoneyAmount = 0,
            RepresentativeName = orderDto.RepresentativeName,
            RepresentativePhone = orderDto.RepresentativePhone
        };
    }

    public static OrderCreatedMessage ToMessage (this Order order)
    {
        return new OrderCreatedMessage
        {
            OrderCode = order.OrderCode,
            OrganizationId = order.OrganizationId.ToString(),
            TotalMoneyAmount = order.TotalMoneyAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            RepresentativeName = order.RepresentativeName,
            RepresentativePhone = order.RepresentativePhone,
            OrderItems = order.OrderItems.Select(OrderItemMappings.ToMessage).ToList(),
            AssignedSystemAdminId = order.AssignedSystemAdminId ?? Guid.Empty
        };
       
    }
}

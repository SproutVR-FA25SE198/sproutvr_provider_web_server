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
            TotalMoneyAmount = order.TotalMoneyAmount,
            TransactionCode = order.TransactionCode,
            PaymentMethod = order.PaymentMethod.ToString(),
            Bank = order.Bank,
            Status = order.Status.ToString(),
            BundleUrl = order.BundleUrl,
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
            TotalMoneyAmount = order.TotalMoneyAmount,
            TransactionCode = order.TransactionCode,
            PaymentMethod = order.PaymentMethod.ToString(),
            Bank = order.Bank,
            Status = order.Status.ToString(),
            BundleUrl = order.BundleUrl,
            CreatedAtUtc = order.CreatedAtUtc,
            UpdatedAtUtc = order.UpdatedAtUtc,
            OrderItems = order.OrderItems?.Select(oi => oi.ToDto()).ToList() ?? new List<OrderItemDto>()
        };
    }

    public static Order ToEntity(this CreateOrderDto orderDto)
    {
        return new Order
        {
            OrganizationId = orderDto.OrganizationId,
            TotalMoneyAmount = 0,
            Status = OrderStatus.Pending_Payment,
        };
    }
}

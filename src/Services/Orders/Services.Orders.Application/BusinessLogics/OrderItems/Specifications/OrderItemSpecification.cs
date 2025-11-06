using Common.Application.Helpers;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
public class OrderItemSpecification : BaseSpecification<OrderItem>
{
    public OrderItemSpecification(OrderItemPurchasedMapParams specParams)
        : base(
            o => o.Order.OrganizationId == specParams.OrganizationId &&
                (string.IsNullOrEmpty(specParams.MapId) || o.MapId.ToString() == specParams.MapId) &&
                (string.IsNullOrEmpty(specParams.MapName) || o.MapName.Contains(specParams.MapName, StringComparison.CurrentCultureIgnoreCase)) &&
                (string.IsNullOrEmpty(specParams.MapCode) || o.MapCode.Contains(specParams.MapCode, StringComparison.CurrentCultureIgnoreCase)) &&
                (!specParams.MinPrice.HasValue || o.Price >= specParams.MinPrice) &&
                (!specParams.MaxPrice.HasValue || o.Price <= specParams.MaxPrice) && 
                (string.IsNullOrEmpty(specParams.SubjectName) || o.SubjectName.Contains(specParams.SubjectName)) &&
            (o.Order.Status == OrderStatus.Bundle_Pending || o.Order.Status == OrderStatus.Finished)
        )
    {
        AddInclude(o => o.Order);
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        AddOrderByDescending(x => x.CreatedAtUtc);
    }

    public OrderItemSpecification(Guid orderId)
        : base(o => o.OrderId == orderId)
    {
        // Don't include Order navigation property to avoid lazy loading issues
        // Just query OrderItems directly
        AddOrderByDescending(x => x.CreatedAtUtc);
    }
}

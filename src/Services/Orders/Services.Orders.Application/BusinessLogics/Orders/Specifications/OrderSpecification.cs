using Common.Application.Helpers;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Specifications;
public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(long OrderCode) : base(o => 
        o.OrderCode == OrderCode 
    )
    {
        AddInclude(o => o.OrderItems);
    }
    public OrderSpecification(Guid id) : base(o => o.Id == id)
    {
        AddInclude(o => o.OrderItems);
    }

    public OrderSpecification(string activationKey)
        : base(o => o.ActivationKey == activationKey)
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0047:Remove unnecessary parentheses", Justification = "<Payment_Pending>")]
    public OrderSpecification(OrderParams orderParams) : base(o =>
    (!orderParams.OrganizationId.HasValue || o.OrganizationId == orderParams.OrganizationId) &&
        (!orderParams.MinAmount.HasValue || o.TotalMoneyAmount >= orderParams.MinAmount) &&
        (!orderParams.MaxAmount.HasValue || o.TotalMoneyAmount <= orderParams.MaxAmount) &&
        (!orderParams.OrderCode.HasValue || (o.OrderCode != null && o.OrderCode == orderParams.OrderCode)) &&
        (string.IsNullOrEmpty(orderParams.Status) || o.Status.ToString() == orderParams.Status) &&
        (!orderParams.FromDate.HasValue || o.CreatedAtUtc >= orderParams.FromDate) &&
        (!orderParams.ToDate.HasValue || o.CreatedAtUtc <= orderParams.ToDate)
    )
    {
        ApplyPaging(orderParams.PageSize * (orderParams.PageIndex - 1), orderParams.PageSize);
        AddOrderByDescending(x => x.CreatedAtUtc);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0047:Remove unnecessary parentheses", Justification = "<Payment_Pending>")]
    public OrderSpecification(OrderHistoryParams orderParams) : base(o =>
    o.OrganizationId == orderParams.OrganizationId &&
        (!orderParams.MinAmount.HasValue || o.TotalMoneyAmount >= orderParams.MinAmount) &&
        (!orderParams.MaxAmount.HasValue || o.TotalMoneyAmount <= orderParams.MaxAmount) &&
        (!orderParams.OrderCode.HasValue || (o.OrderCode != null && o.OrderCode == orderParams.OrderCode)) &&
        (string.IsNullOrEmpty(orderParams.Status) || o.Status.ToString() == orderParams.Status) &&
        (!orderParams.FromDate.HasValue || o.CreatedAtUtc >= orderParams.FromDate) &&
        (!orderParams.ToDate.HasValue || o.CreatedAtUtc <= orderParams.ToDate)
    )
    {
        ApplyPaging(orderParams.PageSize * (orderParams.PageIndex - 1), orderParams.PageSize);
        AddOrderByDescending(x => x.CreatedAtUtc);
    }
}

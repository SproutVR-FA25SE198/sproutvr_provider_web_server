using Common.Application.Helpers;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Specifications;
public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(Guid id) : base(o => o.Id == id)
    {
        AddInclude(o => o.OrderItems);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0047:Remove unnecessary parentheses", Justification = "<Pending>")]
    public OrderSpecification(OrderParams orderParams) : base(o =>
    (!orderParams.OrganizationId.HasValue || o.OrganizationId == orderParams.OrganizationId) &&
        (!orderParams.MinAmount.HasValue || o.TotalMoneyAmount >= orderParams.MinAmount) &&
        (!orderParams.MaxAmount.HasValue || o.TotalMoneyAmount <= orderParams.MaxAmount) &&
        (string.IsNullOrEmpty(orderParams.TransactionCode) || (o.TransactionCode != null && o.TransactionCode.Contains(orderParams.TransactionCode))) &&
        (string.IsNullOrEmpty(orderParams.PaymentMethod) || (!o.PaymentMethod.HasValue && o.PaymentMethod.ToString() == orderParams.PaymentMethod)) &&
        (string.IsNullOrEmpty(orderParams.Bank) || (o.Bank != null && o.Bank.Contains(orderParams.Bank))) &&
        (string.IsNullOrEmpty(orderParams.Status) || o.Status.ToString() == orderParams.Status) &&
        (!orderParams.FromDate.HasValue || o.CreatedAt >= orderParams.FromDate) &&
        (!orderParams.ToDate.HasValue || o.CreatedAt <= orderParams.ToDate)
    )
    {
        ApplyPaging(orderParams.PageSize * (orderParams.PageIndex - 1), orderParams.PageSize);
        AddOrderBy(x => x.CreatedAt);
    }
}

using Common.Domain.Entities;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Domain.Entities.Orders;
public class Order : BaseEntity
{
    public Guid OrganizationId { get; set; } // Organization is from Organizations service
    public decimal TotalMoneyAmount { get; set; }
    public string TransactionCode { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Bank { get; set; }
    public OrderStatus Status { get; set; }
    public string BundleUrl { get; set; }

    // navigation property
    public List<OrderItem> OrderItems { get; set; } = [];
}

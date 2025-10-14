using Common.Domain.Entities;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Domain.Entities.Orders;
public class Order : BaseEntity
{
    public Guid OrganizationId { get; set; } // Organization is from Organizations service
    public decimal TotalMoneyAmount { get; set; }
    public long? PayosOrderCode { get; set; } // for payos
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Bank { get; set; }
    public OrderStatus Status { get; set; }
    public string RepresentativeName { get; set; } // nguoi dai dien mua
    public string RepresentativePhone { get; set; }

    // navigation property
    public List<OrderItem> OrderItems { get; set; } = [];
    public int TotalItems { get; set; }
}

using Common.Domain.Entities;

namespace Services.Orders.Domain.Entities.Orders;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MapId { get; set; } // Map is from Catalogs service
    // navigation property
    public Order Order { get; set; }
}

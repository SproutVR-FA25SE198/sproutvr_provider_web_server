using Common.Domain.Entities;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Domain.Entities.OrderItems;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MapId { get; set; } // Map is from Catalogs service
    // navigation property
    public Order Order { get; set; }
}

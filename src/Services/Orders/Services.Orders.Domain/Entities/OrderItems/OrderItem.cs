using Common.Domain.Entities;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Domain.Entities.OrderItems;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MapId { get; set; } 
    public string MapCode { get; set; }
    public string MapName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public Order Order { get; set; }
}

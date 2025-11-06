using Common.Domain.Entities;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Domain.Entities.OrderItems;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MapId { get; set; } 
    public string MapCode { get; set; } = string.Empty;
    public string MapName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public Order Order { get; set; }
}

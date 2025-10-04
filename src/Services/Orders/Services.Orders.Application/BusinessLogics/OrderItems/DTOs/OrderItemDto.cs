namespace Services.Orders.Application.BusinessLogics.OrderItems.DTOs;
public class OrderItemDto
{
    public Guid OrderId { get; set; }
    public Guid MapId { get; set; } 
    public string? MapName { get; set; }
    public string? MapCode { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }

}

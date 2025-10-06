namespace Services.Orders.Application.BusinessLogics.Basket.DTOs;
public class BasketItemDto
{
    public string MapId { get; set; }
    public string? MapName { get; set; }
    public string? MapCode { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
}

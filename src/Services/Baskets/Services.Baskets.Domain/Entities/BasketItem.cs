namespace Services.Baskets.Domain.Entities;

public class BasketItem
{
    public Guid MapId { get; set; }
    public string? MapName { get; set; }
    public string? MapCode { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? SubjectName { get; set; }
}

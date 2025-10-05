namespace Services.Orders.Application.BusinessLogics.Basket.DTOs;
public class BasketDto
{
    public string Id { get; set; }
    public Guid OrganizationId { get; set; }
    public List<BasketItemDto> BasketItems { get; set; } = [];
}

namespace Services.Baskets.Domain.Entities;
public class Basket
{
    public Basket()
    {
        
    }
    public Basket(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
    public Guid OrganizationId { get; set; }
    public List<BasketItem> BasketItems { get; set; } = [];
}

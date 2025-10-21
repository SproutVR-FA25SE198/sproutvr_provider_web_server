using Common.Application.Helpers;

namespace Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
public class OrderItemPurchasedMapParams : PagingParams
{
    public Guid? OrganizationId { get; set; }
    public string? MapId {get; set;}
    public string? MapName { get; set; }
    public string? MapCode { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SubjectName { get; set; }
}

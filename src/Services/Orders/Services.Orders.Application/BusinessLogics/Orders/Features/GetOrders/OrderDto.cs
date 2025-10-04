namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
public class OrderDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public decimal TotalMoneyAmount { get; set; }
    public string? TransactionCode { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Bank { get; set; }
    public string Status { get; set; }
    public string? BundleUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

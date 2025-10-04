using Common.Application.Helpers;
namespace Services.Orders.Application.BusinessLogics.Orders.Specifications;
public class OrderParams : PagingParams
{
    public string? OrganizationName { get; set; }
    public Guid? OrganizationId { get; set; }
    public decimal? MinAmount { get; set; } 
    public decimal? MaxAmount { get; set; }
    public string? TransactionCode { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Bank { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

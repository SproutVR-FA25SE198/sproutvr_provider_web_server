using Common.Application.Helpers;

namespace Services.Payments.Application.Helpers;
public class PaymentParams : PagingParams
{
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public Guid? OrderId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

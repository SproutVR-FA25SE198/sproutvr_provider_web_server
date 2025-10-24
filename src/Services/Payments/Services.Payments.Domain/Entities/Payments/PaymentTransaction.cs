using Common.Domain.Entities;

namespace Services.Payments.Domain.Entities.Payments;
public class PaymentTransaction : BaseEntity
{
    public decimal Amount { get; set; }
    public Guid OrderId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? TransactionCode { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public PaymentType PaymentType { get; set; }
    public string Description { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionDateTime { get; set; }
    public string Currency { get; set; }

}

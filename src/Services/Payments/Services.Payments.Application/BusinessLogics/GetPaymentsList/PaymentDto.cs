namespace Services.Payments.Application.BusinessLogics.GetPaymentsList;
public class PaymentDto
{
    public decimal Amount { get; set; }
    public Guid OrderId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionCode { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
    public string PaymentType { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAtUtc { get; set; } 
    public string Currency { get; set; }
}

namespace Services.Payments.Application.BusinessLogics.CreatePayment;
public class CreatePaymentDto
{
    public int TotalMoneyAmount { get; set; }
    public string? Description { get; set; }
    public int OrderCode { get; set; }
    public string ReturnUrl { get; set; }
    public string CancelUrl { get; set; }
}

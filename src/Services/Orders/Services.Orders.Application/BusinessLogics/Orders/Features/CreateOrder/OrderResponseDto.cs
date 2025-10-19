namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class OrderResponseDto
{
    public string PaymentUrl { get; set; } = null!;
    public Guid? OrderId { get; set; } 
}

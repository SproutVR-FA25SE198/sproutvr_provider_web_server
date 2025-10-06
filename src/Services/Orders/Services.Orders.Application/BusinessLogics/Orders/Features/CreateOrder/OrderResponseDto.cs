namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class OrderResponseDto
{
    public Guid OrderId { get; set; }
    public decimal TotalMoneyAmount { get; set; }
}

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderResponseDto
{
    public bool IsSuccess { get; set; }
    public Guid? OrderId { get; set; }
}

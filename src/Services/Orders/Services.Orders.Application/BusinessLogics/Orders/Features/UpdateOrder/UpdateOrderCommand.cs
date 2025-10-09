using MediatR;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderCommand: IRequest<bool>
{
    public long OrderCode { get; set; }
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
}

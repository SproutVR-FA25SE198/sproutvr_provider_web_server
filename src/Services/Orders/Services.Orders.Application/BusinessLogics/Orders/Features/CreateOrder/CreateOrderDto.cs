using Services.Orders.Application.BusinessLogics.Basket.DTOs;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderDto
{
    public Guid OrganizationId { get; set; }
    public string PaymentMethod { get; set; }
    public BasketDto Basket {  get; set; }
}

using Services.Orders.Application.BusinessLogics.Basket.DTOs;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderDto
{
    public Guid OrganizationId { get; set; }
    public BasketDto Basket {  get; set; }
}

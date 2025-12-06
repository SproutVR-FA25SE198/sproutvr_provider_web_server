using Services.Orders.Application.BusinessLogics.Basket.DTOs;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderDto
{
    public Guid OrganizationId { get; set; }
    public string RepresentativeName { get; set; }
    public string RepresentativePhone { get; set; }
    public string PaymentMethod { get; set; }
    public BasketDto Basket {  get; set; }
}

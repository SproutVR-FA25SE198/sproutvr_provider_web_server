using Services.Orders.Application.BusinessLogics.OrderItems.DTOs;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderById;
public class OrderDetailsDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public decimal TotalMoneyAmount { get; set; }
    public int TotalItems { get; set; }
    public long? OrderCode { get; set; }
    public string Status { get; set; }
    public string RepresentativeName { get; set; }
    public string RepresentativePhone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public string? ActivationKey { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];
}

using Common.Domain.Entities;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Domain.Entities.Orders;
public class Order : BaseEntity
{
    public Guid OrganizationId { get; set; }            // Organization is from Organizations service
    public decimal TotalMoneyAmount { get; set; }
    public long? OrderCode { get; set; } 
    public OrderStatus Status { get; set; }
    public string? ActivationKey { get; set; }          // Activation key required for verifying bundle import
    public bool IsKeyActivated { get; set; }            // Prevent key from being activated multiple times
    public string RepresentativeName { get; set; }      // nguoi dai dien mua
    public string RepresentativePhone { get; set; }
    public Guid? AssignedSystemAdminId { get; set; }    // SystemAdmin is from Accounts service

    // navigation property
    public List<OrderItem> OrderItems { get; set; } = [];
    public int TotalItems { get; set; }
}

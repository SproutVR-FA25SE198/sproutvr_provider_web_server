namespace Common.Application.Contracts.Orders;
public sealed class OrderCreatedFaultMessage
{
    public long? OrderCode { get; set; }
    public string OrganizationId { get; set; }
    public decimal TotalMoneyAmount { get; set; }
    public string RepresentativeName { get; set; }
    public string RepresentativePhone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; } = [];
    public Guid AssignedSystemAdminId { get; set; }
}

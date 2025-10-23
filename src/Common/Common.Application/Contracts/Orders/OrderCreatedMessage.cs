namespace Common.Application.Contracts.Orders;
public sealed class OrderCreatedMessage
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

public sealed class OrderItemMessage
{
    public Guid MapId { get; set; }
    public string? MapName { get; set; }
    public string? MapCode { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? SubjectName { get; set; }
}

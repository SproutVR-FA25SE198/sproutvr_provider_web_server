namespace Common.Application.Contracts.Orders;
public sealed class OrderCreatedFaultMessage
{
    public long OrderCode { get; set; }
    public string OrganizationId { get; set; }
    public decimal TotalMoneyAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

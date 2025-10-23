namespace Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;
public class SystemAdminDto
{
    public Guid SystemAdminId { get; set; }
    public string FullName { get; set; }
    public int NumberOfPendingOrders { get; set; }
}

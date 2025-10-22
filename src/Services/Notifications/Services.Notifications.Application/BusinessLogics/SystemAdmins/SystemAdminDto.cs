namespace Services.Notifications.Application.BusinessLogics.SystemAdmins;
public class SystemAdminDto
{
    public Guid SystemAdminId { get; set; }
    public string FullName { get; set; }
    public int NumberOfPendingOrders { get; set; }
}

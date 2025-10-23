namespace Services.Bundles.Application.BusinessLogics.UploadBundle;
public class OrderDto
{
    public Guid OrderId { get; set; }
    public Guid OrganizationId { get; set; }
    public string BundleGoogleDriveId { get; set; }
    public Guid AssignedSystemAdminId { get; set; }
}

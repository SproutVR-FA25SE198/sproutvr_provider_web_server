namespace Common.Application.Contracts.Bundles;
public class BundleUploadedFaultMessage
{
    public string OrderId { get; set; }
    public string OrganizationId { get; set; }
    public string AssignedSystemAdminId { get; set; }
}

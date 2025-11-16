namespace Common.Application.Contracts.Bundles;

public class BundleUploadedMessage
{
    public long OrderCode { get; set; }
    public string OrganizationId { get; set; }
    public string AssignedSystemAdminId { get; set; }

}

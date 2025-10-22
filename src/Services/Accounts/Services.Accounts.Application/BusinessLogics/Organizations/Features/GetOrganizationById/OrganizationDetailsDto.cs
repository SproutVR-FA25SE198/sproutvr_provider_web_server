using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;
public class OrganizationDetailsDto : ApplicationUserDto
{
    public string? MACAddress { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? ActivationKey { get; set; }
    public string? BundleGoogleDriveId { get; set; }
}

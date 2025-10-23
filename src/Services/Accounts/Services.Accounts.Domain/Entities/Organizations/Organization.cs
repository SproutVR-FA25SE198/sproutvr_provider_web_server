using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Domain.Entities.Organizations;
public class Organization : ApplicationUser
{
    public string? MACAddress { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? ActivationKey { get; set; } 
    public string? BundleGoogleDriveId { get; set; }
}

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
public class OrganizationDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? MACAddress { get; set; }
    public string? ActivationKey { get; set; }
    public string? BundleGoogleDriveId { get; set; }
}

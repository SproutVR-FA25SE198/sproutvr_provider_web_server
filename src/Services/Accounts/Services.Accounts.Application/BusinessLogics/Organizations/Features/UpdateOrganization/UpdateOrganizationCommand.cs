using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;
public class UpdateOrganizationCommand  : IRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? MACAddress { get; set; }
    public string? BundleGoogleDriveUrl { get; set; }
}

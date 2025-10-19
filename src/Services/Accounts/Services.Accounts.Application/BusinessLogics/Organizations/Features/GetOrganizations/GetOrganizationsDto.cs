using Common.Application.Helpers;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
public class GetOrganizationsDto : PagingParams
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public string? MACAddress { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
}

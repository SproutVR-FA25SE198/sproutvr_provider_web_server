using Common.Application.Helpers;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
public class OrganizationRequestSpecParams : PagingParams
{
    public string? OrganizationName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? ApprovalStatus { get; set; }
}

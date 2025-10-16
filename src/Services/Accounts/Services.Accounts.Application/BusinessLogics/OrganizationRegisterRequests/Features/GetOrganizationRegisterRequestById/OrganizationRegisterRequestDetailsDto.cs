namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequestById;
public class OrganizationRegisterRequestDetailsDto
{
    public string Id { get; set; }
    public string OrganizationName { get; set; }
    public string Address { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string ApprovalStatus { get; set; }
}

using Common.Domain.Entities;

namespace Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
public class OrganizationRegisterRequest : BaseEntity
{
    public string OrganizationName { get; set; }
    public string Address { get; set; } 
    public string ContactPhone { get; set; } 
    public string ContactEmail { get; set; } 
    public string RepresentativeName { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
}

using MediatR;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;
public class CheckOrganizationRegisterRequestCommand : IRequest<bool>
{
    public Guid OrganizationRegisterRequestId { get; set; }
    public string ApprovalStatus { get; set; }
    public string? RejectReason { get; set; }
}

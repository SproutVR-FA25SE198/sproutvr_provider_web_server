using MediatR;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequestById;
public class GetOrganizationRegisterRequestByIdQuery(Guid id) : IRequest<OrganizationRegisterRequestDetailsDto>
{
    public Guid Id { get; set; } = id;
}

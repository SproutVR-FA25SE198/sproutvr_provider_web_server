using Common.Application.Helpers;
using MediatR;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
public class GetOrganizationRegisterRequestsQuery(OrganizationRequestSpecParams specParams) : IRequest<PaginatedResult<OrganizationRegisterRequestDto>>
{
    public OrganizationRequestSpecParams SpecParams { get; set; } = specParams;
}

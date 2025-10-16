using Common.Application.Helpers;
using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
public class GetOrganizationsQuery(GetOrganizationsDto dto) : IRequest<PaginatedResult<OrganizationDto>>
{
    public GetOrganizationsDto Dto { get; set; } = dto;
}

using Common.Application.Helpers;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
using Services.Accounts.Application.BusinessLogics.Organizations.Specifications;
using Services.Accounts.Domain.Entities.Organizations;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
public class GetOrganizationsQueryHandler(IOrganizationRepository organizationRepository) : IRequestHandler<GetOrganizationsQuery, PaginatedResult<OrganizationDto>>
{
    public async Task<PaginatedResult<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Organization> organizations = await organizationRepository.ListAsync(new OrganizationSpecification(request.Dto));
        
        int totalCount = await organizationRepository.CountAsync(new OrganizationSpecification(request.Dto));
        
        List<OrganizationDto> result = organizations.Any() ? organizations.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<OrganizationDto>(request.Dto.PageIndex, request.Dto.PageSize, totalCount, result);
    }
}

using Common.Application.Helpers;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.Organizations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
public class GetOrganizationsQueryHandler(IOrganizationRepository repo) : IRequestHandler<GetOrganizationsQuery, PaginatedResult<OrganizationDto>>
{
    public async Task<PaginatedResult<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Organization> result = await repo.GetAllAsync(); // replace by search and paging logic later
        return new PaginatedResult<OrganizationDto>(
            1,
            result.Count(),
            result.Count(),
            (IReadOnlyList<OrganizationDto?>)result);
    }
}

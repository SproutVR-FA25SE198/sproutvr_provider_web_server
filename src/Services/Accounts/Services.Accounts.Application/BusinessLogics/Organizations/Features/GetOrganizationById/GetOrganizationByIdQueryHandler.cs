using Common.Domain.Exceptions;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
using Services.Accounts.Domain.Entities.Organizations;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;
public class GetOrganizationByIdQueryHandler(IOrganizationRepository repo) : IRequestHandler<GetOrganizationByIdQuery, OrganizationDetailsDto>
{
    public async Task<OrganizationDetailsDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        Organization? org = await repo.GetByIdAsync(request.Id) ?? throw new NotFoundException("Organization not found!");
       
        return OrganizationMappings.ToDetailsDto(org);
    }
}

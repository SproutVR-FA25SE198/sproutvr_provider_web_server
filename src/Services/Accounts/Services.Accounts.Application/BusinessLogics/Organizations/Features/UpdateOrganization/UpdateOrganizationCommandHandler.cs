using Common.Domain.Exceptions;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
using Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.Organizations;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;
public class UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository) : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
{
    public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        Organization existedOrg = await organizationRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException(AppCts.Errors.Organizations.NotFound);

        _ = OrganizationMappings.ToEntity(existedOrg, request);
        organizationRepository.Update(existedOrg);
        await organizationRepository.SaveChangesAsync();

        return OrganizationMappings.ToDto(existedOrg);
    }
}

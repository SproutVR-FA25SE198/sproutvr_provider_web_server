using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
using Services.Accounts.Application.Helpers;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
public class CreateOrganizationCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IOrganizationRepository organizationRepository) : IRequestHandler<CreateOrganizationCommand>
{
    public async Task Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // check if org existed
        bool orgExisted = await organizationRepository.ExistsAsync(o => o.Email == request.Email || o.PhoneNumber == request.PhoneNumber);
        if (orgExisted)
        {
            throw new OperationFailedException(AppCts.Errors.OrganizationRegisterRequests.Duplicated);
        }

        // check if org requested
        OrganizationRegisterRequest existedOrgRequest = await unitOfWork.Repository<OrganizationRegisterRequest>().GetEntityWithSpec(new OrganizationRequestSpecification(request.Email, request.PhoneNumber));
        if (existedOrgRequest != null)
        {
            throw new OperationFailedException(AppCts.Errors.Organizations.Duplicated);
        }

        Organization organization = OrganizationMappings.ToEntity(request);
        organization.UserName = OrganizationAccountHelper.GenerateUserName(request.Email);


        // add new org with initial default password
        IdentityResult result = await userManager.CreateAsync(organization, OrganizationAccountHelper.GeneratePassword(request.Name));

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(organization, AppCts.Roles.Organization);
        }
        else
        {
            throw new OperationFailedException("Failed to add new organization!");
        }

        // send email with org info, including default password, via notification service by using rabbit mq
    }

}

using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Accounts;
using Common.Domain.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
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
    IOrganizationRepository organizationRepository,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateOrganizationCommandHandler> logger) : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
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

        // Generate password before creating user
        string generatedPassword = OrganizationAccountHelper.GeneratePassword(request.Name);

        // Add new org with initial default password
        IdentityResult result = await userManager.CreateAsync(organization, generatedPassword);

        if (!result.Succeeded)
        {
            throw new OperationFailedException("Failed to add new organization!");
        }

        await userManager.AddToRoleAsync(organization, AppCts.Roles.Organization);
        
        // Send email with org info, including default password, via notification service using RabbitMQ
        // MassTransit Outbox pattern will ensure message delivery
        var message = new OrganizationCreatedMessage
        {
            Name = organization.Name,
            Email = organization.Email!,
            OrganizationId = organization.Id.ToString(),
            UserName = organization.UserName!,
            Password = generatedPassword,
            Address = organization.Address,
            PhoneNumber = organization.PhoneNumber
        };

        logger.LogInformation(
            "Publishing OrganizationCreatedMessage for {OrganizationName} ({Email})",
            message.Name,
            message.Email);

        await publishEndpoint.Publish(message, cancellationToken);
        
        logger.LogInformation("OrganizationCreatedMessage saved to outbox");

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return OrganizationMappings.ToDto(organization);

    }

}

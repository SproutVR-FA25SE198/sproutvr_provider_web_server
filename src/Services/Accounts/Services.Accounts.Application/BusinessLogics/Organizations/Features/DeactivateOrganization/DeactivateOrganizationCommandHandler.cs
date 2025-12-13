using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.DeactivateOrganization;

public class DeactivateOrganizationCommandHandler : IRequestHandler<DeactivateOrganizationCommand, bool>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DeactivateOrganizationCommandHandler> _logger;

    public DeactivateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<DeactivateOrganizationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> Handle(DeactivateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // Get organization by ID
        Organization? organization = await _organizationRepository.GetByIdAsync(request.Id);
        
        if (organization == null)
        {
            _logger.LogWarning("Organization with ID {OrganizationId} not found", request.Id);
            throw new OperationFailedException(AppCts.Errors.Organizations.NotFound);
        }

        // Check if already inactive
        if (organization.Status == AccountStatus.Inactive)
        {
            return true;
        }

        // Update status to Inactive
        organization.Status = AccountStatus.Inactive;
        organization.UpdatedAtUtc = DateTime.UtcNow;

        // Update in database using UserManager
        IdentityResult result = await _userManager.UpdateAsync(organization);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to deactivate organization {OrganizationId}: {Errors}", 
                request.Id, 
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new OperationFailedException("Failed to deactivate organization");
        }

        return true;
    }
}


using Common.Application.Abstractions;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganizationProfile;
public class UpdateOrganizationProfileCommandHandler(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager
    ) : IRequestHandler<UpdateOrganizationProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateOrganizationProfileCommand request, CancellationToken cancellationToken)
    {
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();
        
        ApplicationUser? user = await userManager.FindByEmailAsync(userClaims!.Email!) ?? throw new UnauthorizedAccessException();

        if (user is Organization organization)
        {
            if (!string.IsNullOrEmpty(organization.MACAddress))
            {
                throw new OperationFailedException("You can only update MAC address once!");
            } 
            organization.MACAddress = request.MACAdress;
            IdentityResult result = await userManager.UpdateAsync(organization);
            return result.Succeeded;
        }
        return false;
    }
}

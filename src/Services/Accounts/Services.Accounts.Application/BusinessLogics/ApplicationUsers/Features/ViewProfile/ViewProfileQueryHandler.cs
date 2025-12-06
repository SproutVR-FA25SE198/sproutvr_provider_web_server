using Common.Application.Abstractions;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Mappings;
using Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
using Services.Accounts.Application.BusinessLogics.SystemAdmins;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;
public class ViewProfileQueryHandler(
    UserManager<ApplicationUser> userManager,
    IUserContext userContext) : IRequestHandler<ViewProfileQuery, ApplicationUserDto>
{
    public async Task<ApplicationUserDto> Handle(ViewProfileQuery request, CancellationToken cancellationToken)
    {
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();
        ApplicationUser? user = await userManager.FindByEmailAsync(userClaims!.Email!);

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), user!.Email!);
        }

        if (user is Organization organization)
        {
            return OrganizationMappings.ToDetailsDto(organization);
        }
        else if (user is SystemAdmin systemAdmin)
        {
            return SystemAdminMappings.ToDto(systemAdmin);
        }

        return ApplicationUserMappings.ToDto(user);
    }
}

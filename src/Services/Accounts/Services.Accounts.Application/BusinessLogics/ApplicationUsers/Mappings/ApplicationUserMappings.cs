using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Mappings;
public static class ApplicationUserMappings
{
    public static ApplicationUserDto ToDto(this ApplicationUser user)
    {
        var dto = new ApplicationUserDto
        {
            //Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
        };
        return dto;
    }
}

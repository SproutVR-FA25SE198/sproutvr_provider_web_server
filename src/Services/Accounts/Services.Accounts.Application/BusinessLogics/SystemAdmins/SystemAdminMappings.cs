using Services.Accounts.Domain.Entities.SystemAdmins;

namespace Services.Accounts.Application.BusinessLogics.SystemAdmins;
public static class SystemAdminMappings
{
    public static SystemAdminDto ToDto(this SystemAdmin user)
    {
        var dto = new SystemAdminDto
        {
            //Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            FullName = user.FullName,
        };
        return dto;
    }
}

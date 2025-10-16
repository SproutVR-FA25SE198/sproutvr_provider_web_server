using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;

namespace Services.Accounts.Application.BusinessLogics.SystemAdmins;
public class SystemAdminDto : ApplicationUserDto
{
    public string FullName { get; set; }
}

using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Domain.Entities.SystemAdmins;
public class SystemAdmin : ApplicationUser
{
    public string FullName { get; set; }
}

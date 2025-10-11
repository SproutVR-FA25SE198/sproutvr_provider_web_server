using Microsoft.AspNetCore.Identity;

namespace Services.Accounts.Domain.Entities.UserAccounts;
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}

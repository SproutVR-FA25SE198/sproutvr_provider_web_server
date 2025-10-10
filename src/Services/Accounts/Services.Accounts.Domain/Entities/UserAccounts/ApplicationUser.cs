using Microsoft.AspNetCore.Identity;

namespace Services.Accounts.Domain.Entities.UserAccounts;

public class ApplicationUser : IdentityUser<Guid>
{
    public string AvatarUrl { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

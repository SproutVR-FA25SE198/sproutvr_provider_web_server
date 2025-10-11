using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Services.Accounts.Application.Abstractions.Data.Seeders;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.UserAccounts;
using Services.Accounts.Infrastructure.Data.SeederFiles;

namespace Services.Accounts.Infrastructure.Services;
public class AccountSeeder(
    ILogger<AccountSeeder> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IAccountSeeder
{
    public async Task SeedAsync()
    {
        string[] roles = new[] { AppCts.Roles.SystemAdmin, AppCts.Roles.Organization };

        // 1️⃣ Seed roles
        foreach (string roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }

        // 2️⃣ Seed users
        IEnumerable<ApplicationUser> users = IdentityData.GetUsers();

        foreach (ApplicationUser user in users)
        {
            #pragma warning disable CS8604 // Possible null reference argument.
            if (await userManager.FindByNameAsync(user.UserName) == null)
            {
                IdentityResult result = await userManager.CreateAsync(user, $"{user.UserName}@123");

                if (result.Succeeded)
                {
                    if (user is SystemAdmin)
                    {
                        await userManager.AddToRoleAsync(user, AppCts.Roles.SystemAdmin);
                    }
                    else if (user is Organization)
                    {
                        await userManager.AddToRoleAsync(user, AppCts.Roles.Organization);
                    }
                }
                else
                {
                    logger.LogError("Failed to create users");
                }
            }
            #pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Infrastructure.Helpers;
public static class RoleHelper
{
    public static async Task AddInitialRoles (IServiceScope scope) 
    {
        RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        if (!await roleManager.RoleExistsAsync(AppCts.Roles.SystemAdmin))
        {
            var role = new ApplicationRole { Name = AppCts.Roles.SystemAdmin };
            await roleManager.CreateAsync(role);
        }
        if (!await roleManager.RoleExistsAsync(AppCts.Roles.Organization))
        {
            var role = new ApplicationRole { Name = AppCts.Roles.Organization };
            await roleManager.CreateAsync(role);
        }
    }
}

using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Infrastructure.Data.SeederFiles;
public static class IdentityData
{
    public static IEnumerable<ApplicationUser> GetUsers()
    {
        var users = new List<ApplicationUser>
        {
            // system admins
            new SystemAdmin
            {
                UserName = "systemAdmin1",
                NormalizedUserName = "SYSTEMADMIN1",
                Email = "systemAdmin1@example.com",
                NormalizedEmail = "SYSTEMADMIN1@EXAMPLE.COM",
                FullName = "System Admin One",
                Status = AccountStatus.Active,
                AvatarUrl = "https://systemadmin1.png",
                PhoneNumber = "+84123456789"
            },
            new SystemAdmin
            {
                UserName = "systemAdmin2",
                NormalizedUserName = "SYSTEMADMIN2",
                Email = "systemAdmin2@example.com",
                NormalizedEmail = "SYSTEMADMIN2@EXAMPLE.COM",
                FullName = "System Admin Two",
                Status = AccountStatus.Active,
                AvatarUrl = "https://systemadmin2.png",
                PhoneNumber = "+84123456788"
            },

            // organizations
            new Organization
            {
                Id = Guid.Parse("c0000001-0000-0000-0000-000000000001"),
                UserName = "Organization1",
                NormalizedUserName = "ORGANIZATION1",
                Email = "organization1@example.com",
                NormalizedEmail = "ORGANIZATION1@EXAMPLE.COM",
                Status = AccountStatus.Active,
                AvatarUrl = "https://organization1.png",
                PhoneNumber = "+84987654321",
                Name = "Truong THPT ABC",
                Address = "123, pho XYZ",
                BundleGoogleDriveId = "1a3uWvJIfAGWs5GJ6Gt7hI_K6594RhKsG"
            },
            new Organization
            {
                Id = Guid.Parse("c0000001-0000-0000-0000-000000000002"),
                UserName = "Organization2",
                NormalizedUserName = "ORGANIZATION2",
                Email = "organization2@example.com",
                NormalizedEmail = "ORGANIZATION2@EXAMPLE.COM",
                Status = AccountStatus.Active,
                AvatarUrl = "https://organization2.png",
                PhoneNumber = "+84887654320",
                Name = "Truong THPT DEF",
                Address = "103, pho LMN",
                BundleGoogleDriveId = "1kUy2wfTnbuv8fNEOJec71ym2SYzaC2zq"
            }
        };
        return users;
    }

}

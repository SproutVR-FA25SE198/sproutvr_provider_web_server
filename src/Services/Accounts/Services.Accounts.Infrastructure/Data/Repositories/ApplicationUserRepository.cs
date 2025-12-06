using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.UserAccounts;
using Services.Accounts.Infrastructure.Data.Database;

namespace Services.Accounts.Infrastructure.Data.Repositories;
public class ApplicationUserRepository : GenericIdentityRepository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(AccountDbContext context) : base(context)
    {
    }
}

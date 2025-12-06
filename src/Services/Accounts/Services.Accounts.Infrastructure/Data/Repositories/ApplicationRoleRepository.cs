using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.UserAccounts;
using Services.Accounts.Infrastructure.Data.Database;

namespace Services.Accounts.Infrastructure.Data.Repositories;
public class ApplicationRoleRepository : GenericIdentityRepository<ApplicationRole>, IApplicationRoleRepository
{
    public ApplicationRoleRepository(AccountDbContext context) : base(context)
    {
    }
}

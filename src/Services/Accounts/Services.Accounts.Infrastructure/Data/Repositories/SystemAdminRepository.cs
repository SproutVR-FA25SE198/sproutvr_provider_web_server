using Microsoft.EntityFrameworkCore;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Infrastructure.Data.Database;

namespace Services.Accounts.Infrastructure.Data.Repositories;
public class SystemAdminRepository : GenericIdentityRepository<SystemAdmin>, ISystemAdminRepository
{
    public SystemAdminRepository(AccountDbContext context) : base(context)
    {
    }

    public async Task<SystemAdmin?> GetSystemAdminWithMinPendingOrdersAsync()
    {
        return await _dbSet
            .OrderBy(sa => sa.NumberOfPendingOrders)
            .FirstOrDefaultAsync();
    }
}

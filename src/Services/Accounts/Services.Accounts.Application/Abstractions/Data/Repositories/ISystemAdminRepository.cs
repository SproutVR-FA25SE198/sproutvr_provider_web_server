using Services.Accounts.Domain.Entities.SystemAdmins;

namespace Services.Accounts.Application.Abstractions.Data.Repositories;
public interface ISystemAdminRepository : IGenericIdentityRepository<SystemAdmin>
{
    Task<SystemAdmin?> GetSystemAdminWithMinPendingOrdersAsync();
}

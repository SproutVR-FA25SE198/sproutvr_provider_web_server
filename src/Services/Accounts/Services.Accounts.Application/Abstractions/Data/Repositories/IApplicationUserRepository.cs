using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.Abstractions.Data.Repositories;
public interface IApplicationUserRepository : IGenericIdentityRepository<ApplicationUser>
{

}

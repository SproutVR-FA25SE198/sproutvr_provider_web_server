using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Infrastructure.Data.Database;

namespace Services.Accounts.Infrastructure.Data.Repositories;
public class OrganizationRepository : GenericIdentityRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(AccountDbContext context) : base(context)
    {
    }
}

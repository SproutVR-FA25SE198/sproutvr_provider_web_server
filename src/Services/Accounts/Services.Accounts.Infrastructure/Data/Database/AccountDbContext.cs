using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Services.Accounts.Domain.Entities.UserAccounts;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace Services.Accounts.Infrastructure.Data.Database;
public class AccountDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AccountDbContext()
    {
    }

    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<SystemAdmin> SystemAdmins { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationRegisterRequest> OrganizationRegisterRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Add Outbox Pattern
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}

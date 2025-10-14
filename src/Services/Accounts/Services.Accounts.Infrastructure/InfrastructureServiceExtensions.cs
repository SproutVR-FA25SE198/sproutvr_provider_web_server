using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Common.Infrastructure.Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.Abstractions.Data.Seeders;
using Services.Accounts.Application.Abstractions.Services;
using Services.Accounts.Domain.Entities.UserAccounts;
using Services.Accounts.Infrastructure.Data.Database;
using Services.Accounts.Infrastructure.Data.Repositories;
using Services.Accounts.Infrastructure.Services;

namespace Services.Accounts.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Postgres
        services.AddDbContext<AccountDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        // Add Identity
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders();

        // Add Unit Of Work & Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<AccountDbContext>>();

        services.AddScoped(typeof(IGenericIdentityRepository<>), typeof(GenericIdentityRepository<>));
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();


        // Add Helper Services
        services.AddScoped<ITokenService, TokenService>();

        // Add Seeding
        services.AddScoped<AccountDbContextSeeder>();
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IDataSeeder, JsonDataSeeder<AccountDbContext>>();
        services.AddScoped<IAccountSeeder, AccountSeeder>();

        return services;
    }
}

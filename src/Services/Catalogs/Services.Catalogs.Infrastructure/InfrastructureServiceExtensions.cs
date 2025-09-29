using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Common.Infrastructure.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Catalogs.Infrastructure.Data.Database;

namespace Services.Catalogs.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Postgres
        services.AddDbContext<CatalogDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        // Add Unit Of Work & Generic Repository
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork<CatalogDbContext>>();

        // Add Seeding
        services.AddScoped<CatalogDbContextSeeder>();
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IDataSeeder, JsonDataSeeder<CatalogDbContext>>();

        return services;
    }
}

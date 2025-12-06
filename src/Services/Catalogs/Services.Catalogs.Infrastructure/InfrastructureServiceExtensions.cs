using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Common.Infrastructure.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Catalogs.Application.Abstractions.Grpc.Clients;
using Services.Catalogs.Application.Abstractions.Services;
using Services.Catalogs.Infrastructure.Data.Database;
using Services.Catalogs.Infrastructure.Services;
using Services.Catalogs.Infrastructure.Services.Grpc.Client;

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
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<CatalogDbContext>>();

        // Add Seeding
        services.AddScoped<CatalogDbContextSeeder>();
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IDataSeeder, JsonDataSeeder<CatalogDbContext>>();

        // Add Metadata Generation Service
        services.AddScoped<IMapMetadataService, MapMetadataService>();

        // Add gRPC Client wrappers
        services.AddScoped<IBundleGrpcClient, BundleGrpcClient>();

        return services;
    }
}

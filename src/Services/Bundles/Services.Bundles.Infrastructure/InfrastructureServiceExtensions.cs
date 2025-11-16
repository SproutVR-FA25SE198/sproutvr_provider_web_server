using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Bundles.Application.Abstractions.Services;
using Services.Bundles.Infrastructure.Data.Database;
using Services.Bundles.Infrastructure.Helpers;
using Services.Bundles.Infrastructure.Services;

namespace Services.Bundles.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BundleDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        // Add Unit Of Work & Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<BundleDbContext>>();

        services.AddScoped<OAuthHelper>();
        services.AddScoped<IGoogleDriveService, GoogleDriveService>();

        return services;

    }
}

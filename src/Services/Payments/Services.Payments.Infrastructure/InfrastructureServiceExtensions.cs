using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Infrastructure.Data.Database;
using Services.Payments.Infrastructure.Services;
using Services.Payments.Infrastructure.Services.Grpc.Client;

namespace Services.Payments.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Postgres
        services.AddDbContext<PaymentDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        // Add Unit Of Work & Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<PaymentDbContext>>();

        // Add Payos PaymentTransaction Service
        services.AddScoped<IPayosPaymentService, PayosPaymentService>();

        // Add Grpc Clients
        services.AddScoped<IGrpcOrderClient, GrpcOrderClient>();

        return services;
    }
}

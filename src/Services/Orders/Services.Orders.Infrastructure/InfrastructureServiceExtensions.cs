using Common.Application.Abstractions;
using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Common.Infrastructure.Data.Seeders;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Accounts.Infrastructure.Services;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Infrastructure.Data.Database;
using Services.Orders.Infrastructure.Services.Grpc.Client;

namespace Services.Orders.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Postgres
        services.AddDbContext<OrderDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        // Add Unit Of Work & Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<OrderDbContext>>();

        // user context
        services.AddScoped<IUserContext, UserContext>();

        // Add Seeding
        services.AddScoped<OrderDbContextSeeder>();
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IDataSeeder, JsonDataSeeder<OrderDbContext>>();
        
        // Add Grpc Clients
        services.AddScoped<IGrpcMapClient, GrpcMapClient>();
        services.AddScoped<IGrpcPaymentClient, GrpcPaymentClient>();
        services.AddScoped<IGrpcAccountClient, GrpcAccountClient>();

        return services;
    }
}

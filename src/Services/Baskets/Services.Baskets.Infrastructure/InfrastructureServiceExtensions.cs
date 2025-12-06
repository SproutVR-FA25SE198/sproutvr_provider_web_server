using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Infrastructure.Data;
using StackExchange.Redis;

namespace Services.Baskets.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Redis
        services.AddSingleton<IConnectionMultiplexer>(opt =>
        {
            string connString = configuration.GetConnectionString("Redis")
                ?? throw new Exception("Cannot get redis connection string");
            return ConnectionMultiplexer.Connect(connString);
        });

        services.AddScoped<IBasketRepository, BasketRepository>();

        return services;
    }
}

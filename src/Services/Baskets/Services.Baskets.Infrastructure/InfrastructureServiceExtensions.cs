using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Infrastructure.Data;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        return services;
    }
}

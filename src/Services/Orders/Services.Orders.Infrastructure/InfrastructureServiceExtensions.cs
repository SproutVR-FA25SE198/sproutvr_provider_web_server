using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using Common.Infrastructure.Data.Seeders;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Orders.Infrastructure.Data.Database;

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
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork<OrderDbContext>>();

        // Add Seeding
        services.AddScoped<OrderDbContextSeeder>();
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IDataSeeder, JsonDataSeeder<OrderDbContext>>();

        return services;
    }
}

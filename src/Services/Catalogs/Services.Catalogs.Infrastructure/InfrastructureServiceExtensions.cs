using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data;
using FluentValidation;
using MassTransit;
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
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<,>).MakeGenericType(typeof(CatalogDbContext)));
        services.AddScoped<IUnitOfWork, UnitOfWork<CatalogDbContext>>();

        return services;
    }
}

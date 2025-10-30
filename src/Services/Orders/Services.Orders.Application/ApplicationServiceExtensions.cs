using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Services.Orders.Application.Abstractions.Services;
namespace Services.Orders.Application;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add Fluent Validator
        services.AddValidatorsFromAssembly(ApplicationReference.Assembly, includeInternalTypes: true);

        // Add Mediator
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(ApplicationReference.Assembly);
        });

        // Add gRPC Services
        services.AddGrpc();

        return services;
    }
}

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Catalogs.Application;
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

        // Add Grpc Server
        services.AddGrpc();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Services.Bundles.Application;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add Mediator
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(ApplicationReference.Assembly);
        });

        // Add gRPC Server
        services.AddGrpc();

        return services;
    }
}


using Common.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
namespace Services.Payments.Application;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        // Add gRPC Services
        services.AddGrpc();

        // Add Mediator
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(ApplicationReference.Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}

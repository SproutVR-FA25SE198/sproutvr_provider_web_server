using Microsoft.Extensions.DependencyInjection;
namespace Services.Payments.Application;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        // Add gRPC Services
        services.AddGrpc();

        return services;
    }
}

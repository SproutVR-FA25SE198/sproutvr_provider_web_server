using Microsoft.Extensions.DependencyInjection;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.Abstractions.Grpc.Client;
using Services.Payments.Infrastructure.Services;
using Services.Payments.Infrastructure.Services.Grpc.Client;

namespace Services.Payments.Infrastructure.Extensions;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Add Payos Payment Service
        services.AddScoped<IPayosPaymentService, PayosPaymentService>();

        // Add Grpc Clients
        services.AddScoped<IGrpcOrderClient, GrpcOrderClient>();

        return services;
    }
}

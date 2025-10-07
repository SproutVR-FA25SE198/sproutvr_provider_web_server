using Microsoft.Extensions.DependencyInjection;
using Services.Payments.Application.Abstractions;
using Services.Payments.Infrastructure.Services;

namespace Services.Payments.Infrastructure.Extensions;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register PaymentServiceFactory with generic type parameters
        services.AddScoped<IPayosPaymentService,PayosPaymentService>();
        return services;
    }
}

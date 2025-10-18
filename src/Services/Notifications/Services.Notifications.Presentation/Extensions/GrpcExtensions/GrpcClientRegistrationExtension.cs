using Common.Application.Helpers.Grpc;
using Grpc.Core;
using Grpc.Net.Client.Configuration;

namespace Services.Notifications.Presentation.Extensions.GrpcExtensions;

#pragma warning disable CA1515 // Consider making public types internal
public static class GrpcClientRegistrationExtension
#pragma warning restore CA1515 // Consider making public types internal
{
    public static IServiceCollection AddConfiguredGrpcClient<T>(
            this IServiceCollection services,
            string address,
            MethodConfig? methodConfig = null
        ) where T : ClientBase<T>
    {
        services.AddGrpcClient<T>(options =>
        {
            // Set gRPC service base address
            options.Address = new Uri(address);
        })
        .ConfigureChannel(c =>
        {
            // Apply retry and method config policy
            c.ServiceConfig = new ServiceConfig
            {
                MethodConfigs = { methodConfig ?? GrpcRetryPolicy.GetDefaultMethodConfig() }
            };
        });

        return services;
    }
}

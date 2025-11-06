using Common.Presentation.Middlewares;
using OrdersService;
using Services.Bundles.Presentation.Extensions.GrpcExtensions;

namespace Services.Bundles.Presentation.Extensions;

#pragma warning disable CA1515 // Consider making public types internal
public static class WebApplicationBuilderExtensions
#pragma warning restore CA1515 // Consider making public types internal
{
    public static void AddPresentation(this WebApplicationBuilder builder, IConfiguration config)
    {
        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        
        #pragma warning disable CS8604 // Possible null reference argument.
        builder.Services.AddConfiguredGrpcClient<GrpcOrder.GrpcOrderClient>(config["GrpcOrders"]);
        #pragma warning restore CS8604 // Possible null reference argument.
    }
}


using CatalogsService;
using PaymentsService;
using Common.Presentation.Middlewares;
using Services.Orders.Presentation.Extensions.GrpcExtensions;

namespace Services.Orders.Presentation.Extensions;

#pragma warning disable CA1515 // Consider making public types internal
public static class WebApplicationBuilderExtensions
#pragma warning restore CA1515 // Consider making public types internal
{
    public static void AddPresentation(this WebApplicationBuilder builder, IConfiguration config)
    {
        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        
        #pragma warning disable CS8604 // Possible null reference argument.
        
        builder.Services.AddConfiguredGrpcClient<GrpcMap.GrpcMapClient>(config["GrpcMap"]);
        builder.Services.AddConfiguredGrpcClient<GrpcPayment.GrpcPaymentClient>(config["GrpcPayment"]);
        
        #pragma warning restore CS8604 // Possible null reference argument.
    }
}

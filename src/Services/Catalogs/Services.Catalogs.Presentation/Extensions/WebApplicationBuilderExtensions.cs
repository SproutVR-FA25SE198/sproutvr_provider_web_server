using Common.Presentation.Middlewares;

namespace Services.Catalogs.Presentation.Extensions;

#pragma warning disable CA1515 // Consider making public types internal
public static class WebApplicationBuilderExtensions
#pragma warning restore CA1515 // Consider making public types internal
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ErrorHandlingMiddleware>();
    }
}


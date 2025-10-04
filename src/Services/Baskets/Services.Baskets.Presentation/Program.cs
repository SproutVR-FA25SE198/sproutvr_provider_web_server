using Common.Presentation.Middlewares;
using MassTransit;
using Services.Baskets.Application;
using Services.Baskets.Infrastructure;
using Services.Baskets.Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

builder.Services.AddControllers();
builder.AddPresentation();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("baskets", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/",
            h =>
            {
                h.Username(builder.Configuration.GetValue<string>("RabbitMq:Username", "guest"));
                h.Password(builder.Configuration.GetValue<string>("RabbitMq:Password", "guest"));
            });
        cfg.ConfigureEndpoints(context);
    });
});

WebApplication app = builder.Build();

// ==========================
// === Middlewares
// ==========================
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

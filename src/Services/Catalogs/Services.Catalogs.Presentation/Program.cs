using MassTransit;
using Services.Catalogs.Application;
using Services.Catalogs.Infrastructure;
using Services.Catalogs.Presentation.Consumers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add MassTransit wihh Outbox Pattern
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<MapCreatedFaultMessageConsumer>();

    // do not include namespace in the queue name
    // 
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("catalogs", false));
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

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

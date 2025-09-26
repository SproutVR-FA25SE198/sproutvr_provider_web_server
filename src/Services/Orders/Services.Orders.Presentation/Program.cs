using MassTransit;
using Services.Orders.Presentation.Consumers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

builder.Services.AddControllers();

// Add MassTransit wihh Outbox Pattern
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<OrderCreatedFaultMessageConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("orders", false));
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

app.MapControllers();

await app.RunAsync();

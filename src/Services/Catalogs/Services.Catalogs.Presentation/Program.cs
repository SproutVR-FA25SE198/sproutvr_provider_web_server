using MassTransit;
using Services.Catalogs.Application;
using Services.Catalogs.Infrastructure;
using Services.Catalogs.Infrastructure.Data.Database;
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
    x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<MapCreatedFaultMessageConsumer>();
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

app.MapControllers();

// =============================
// === Scoped Service for using
// =============================

// Seeding Data
using (IServiceScope scope = app.Services.CreateScope())
{
    CatalogDbContextSeeder seeder = scope.ServiceProvider.GetRequiredService<CatalogDbContextSeeder>();
    await seeder.SeedAsync();
}

await app.RunAsync();

using Common.Application.Contracts.Accounts;
using Common.Application.Contracts.Bundles;
using Common.Presentation.Middlewares;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Services.Bundles.Application;
using Services.Bundles.Application.Helpers;
using Services.Bundles.Infrastructure;
using Services.Bundles.Infrastructure.Data.Database;
using Services.Bundles.Infrastructure.Services.Grpc.Server;
using Services.Bundles.Presentation;
using Services.Bundles.Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to allow large file uploads (2 GB)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 2147483648; // 2 GB in bytes
});

// ==========================
// === Build Services
// ==========================

builder.Services.AddControllers();
builder.AddPresentation(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add MassTransit wihh Outbox Pattern
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<BundleDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<BundleUploadedFaultMessage>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bundles", false));
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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<GoogleDriveSettings>(builder.Configuration.GetSection("GoogleDrive"));

WebApplication app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Map gRPC Service
app.MapGrpcService<BundleGrpcService>();

using IServiceScope scope = app.Services.CreateScope();

IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
BundleDbContext dbContext = scope.ServiceProvider.GetRequiredService<BundleDbContext>();

if (env.IsDevelopment())
{
    // Development: drop DB, apply migrations, seed all test data
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
}
else if (env.IsStaging())
{
    // Staging: apply migrations, seed only essential reference/lookup data
    await dbContext.Database.MigrateAsync();
}
else if (env.IsProduction())
{
    // Production: apply migrations safely, no DB drop, seed only critical reference data
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();

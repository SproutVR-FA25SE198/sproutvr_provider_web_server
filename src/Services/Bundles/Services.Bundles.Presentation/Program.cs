using Common.Presentation.Middlewares;
using MassTransit;
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Map gRPC Service
app.MapGrpcService<BundleGrpcService>();

await app.RunAsync();

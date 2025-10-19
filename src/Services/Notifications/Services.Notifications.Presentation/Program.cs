using MassTransit;
using OrganizationAccountsService;
using Services.Notifications.Application;
using Services.Notifications.Infrastructure;
using Services.Notifications.Infrastructure.Helpers;
using Services.Notifications.Presentation.Consumers;
using Services.Notifications.Presentation.Extensions.GrpcExtensions;
using Services.Notifications.Presentation.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Allow any origin for development
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // SignalR requires credentials
    });
});

// grpc client
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddConfiguredGrpcClient<GrpcOrganization.GrpcOrganizationClient>(builder.Configuration["GrpcAccount"]);
#pragma warning restore CS8604 // Possible null reference argument.


// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<OrderCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<OrganizationCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<OrganizationRegisterRequestApprovedConsumer>();
    x.AddConsumersFromNamespaceContaining<OrganizationRegisterRequestRejectedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notifications", false));
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

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSignalR();

WebApplication app = builder.Build();

// ==========================
// === Middlewares
// ==========================

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notifications");

await app.RunAsync();

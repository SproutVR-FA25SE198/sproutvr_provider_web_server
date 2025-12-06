using AccountsService;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrganizationAccountsService;
using Services.Notifications.Application;
using Services.Notifications.Infrastructure;
using Services.Notifications.Infrastructure.Helpers;
using Services.Notifications.Presentation.Consumers;
using Services.Notifications.Presentation.Extensions.GrpcExtensions;
using Services.Notifications.Presentation.Hubs;
using System.Text;

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
builder.Services.AddConfiguredGrpcClient<GrpcAccount.GrpcAccountClient>(builder.Configuration["GrpcAccount"]);
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

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
#pragma warning disable CS8604 // Possible null reference argument.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudiences = builder.Configuration.GetSection("JWT:Audiences").Get<List<string>>(),
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
#pragma warning restore CS8604 // Possible null reference argument.

    // Configure SignalR authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Microsoft.Extensions.Primitives.StringValues accessToken = context.Request.Query["access_token"];
            PathString path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// ==========================
// === Middlewares
// ==========================

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notifications");

await app.RunAsync();

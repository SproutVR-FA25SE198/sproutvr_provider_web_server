using System.Text;
using Common.Presentation.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Catalogs.Application;
using Services.Catalogs.Infrastructure;
using Services.Catalogs.Infrastructure.Data.Database;
using Services.Catalogs.Infrastructure.Services.Grpc.Server;
using Services.Catalogs.Presentation.Consumers;
using Services.Catalogs.Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
)
   .AddJwtBearer(options =>
   {
       options.SaveToken = true;
#pragma warning disable CS8604 // Possible null reference argument.
       List<string> audiences = builder.Configuration.GetSection("JWT:Audiences").Get<List<string>>();
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidAudiences = audiences,
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
       };
#pragma warning restore CS8604 // Possible null reference argument.
   }
);

builder.Services.AddAuthorization();
WebApplication app = builder.Build();

// ==========================
// === Middlewares
// ==========================

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// map grpc services
app.MapGrpcService<GrpcMapService>();

// =============================
// === Scoped Service
// =============================

using IServiceScope scope = app.Services.CreateScope();

IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
CatalogDbContext dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
CatalogDbContextSeeder seeder = scope.ServiceProvider.GetRequiredService<CatalogDbContextSeeder>();

if (env.IsDevelopment())
{
    // Development: drop DB, apply migrations, seed all test data
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    await seeder.SeedProductionAsync();
}
else if (env.IsStaging())
{
    // Staging: apply migrations, seed only essential reference/lookup data
    await dbContext.Database.MigrateAsync();
    await seeder.SeedStagingAsync();
}
else if (env.IsProduction())
{
    // Production: apply migrations safely, no DB drop, seed only critical reference data
    await dbContext.Database.MigrateAsync();
    await seeder.SeedProductionAsync();
}

await app.RunAsync();

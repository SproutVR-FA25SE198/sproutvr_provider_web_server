using System.Text;
using Common.Presentation.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Accounts.Application;
using Services.Accounts.Infrastructure;
using Services.Accounts.Infrastructure.Data.Database;
using Services.Accounts.Infrastructure.Services.Grpc;
using Services.Accounts.Presentation.Consumers;
using Services.Accounts.Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.AddPresentation();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AccountDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<OrganizationRegisterRequestApprovedFaultMessageConsumer>();
    x.AddConsumersFromNamespaceContaining<OrganizationRegisterRequestRejectedFaultMessageConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("accounts", false));
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", b =>
    {
        b.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(builder.Configuration["ClientApp"]!);
    });
});


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("customPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<CurrentUserMiddleware>();

// map grpc services
app.MapGrpcService<GrpcOrganizationService>();

using IServiceScope scope = app.Services.CreateScope();

IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
AccountDbContext dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
AccountDbContextSeeder seeder = scope.ServiceProvider.GetRequiredService<AccountDbContextSeeder>();

if (env.IsDevelopment())
{
    // Development: drop DB, apply migrations, seed all test data
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    await seeder.SeedDevelopmentAsync();
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

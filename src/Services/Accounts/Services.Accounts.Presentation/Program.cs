using System.Text;
using Common.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Accounts.Application;
using Services.Accounts.Infrastructure;
using Services.Accounts.Infrastructure.Data.Database;
using Services.Accounts.Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.AddPresentation();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
       options.RequireHttpsMetadata = false;
    #pragma warning disable CS8604 // Possible null reference argument.
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidAudience = builder.Configuration["JWT:Audience"],
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
       };
    #pragma warning restore CS8604 // Possible null reference argument.
   }
);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

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

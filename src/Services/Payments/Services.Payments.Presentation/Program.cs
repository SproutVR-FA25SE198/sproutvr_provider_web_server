using Microsoft.OpenApi.Models;
using Net.payOS;
using Services.Payments.Application;
using Services.Payments.Infrastructure.Extensions;
using Services.Payments.Infrastructure.Services.Grpc.Server;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<PayOS>(provider =>
{
    // Create and return the PayOS instance.
#pragma warning disable CS8604 // Possible null reference argument.
    return new PayOS(
        builder.Configuration["PayOs:ClientId"],
        builder.Configuration["PayOs:ApiKey"],
        builder.Configuration["PayOs:ChecksumKey"]
    );
#pragma warning restore CS8604 // Possible null reference argument.
});

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment API", Version = "v.1.0" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

// map grpc services
app.MapGrpcService<GrpcPaymentService>();

await app.RunAsync();

using Common.Presentation.Middlewares;
using MassTransit;
using Net.payOS;
using OrdersService; // grpc service
using Services.Payments.Application;
using Services.Payments.Infrastructure;
using Services.Payments.Infrastructure.Data.Database;
using Services.Payments.Infrastructure.Services.Grpc.Server;
using Services.Payments.Presentation.Extensions.GrpcExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Add MassTransit wihh Outbox Pattern
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<PaymentDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("payments", false));
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

// configure grpc clients
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddConfiguredGrpcClient<GrpcOrder.GrpcOrderClient>(builder.Configuration["GrpcOrder"]);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSingleton<PayOS>(provider =>
{
    // Create and return the PayOS instance.
    return new PayOS(
        builder.Configuration["PayOs:ClientId"],
        builder.Configuration["PayOs:ApiKey"],
        builder.Configuration["PayOs:ChecksumKey"]
    );
#pragma warning restore CS8604 // Possible null reference argument.
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

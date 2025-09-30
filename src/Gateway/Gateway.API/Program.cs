WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

// Configure Yarp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Configure OpenTelemetry (LATER)

// Configure Authentication JWT Bearer Token (LATER)

// Configure CORS
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


// ==========================
// === Middlewares
// ==========================

app.UseCors("customPolicy");
app.MapReverseProxy();

await app.RunAsync();

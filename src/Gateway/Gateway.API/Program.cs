using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ==========================
// === Build Services
// ==========================

// Configure Yarp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Configure OpenTelemetry (LATER)

// Configure Authentication JWT Bearer Token (LATER)
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
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

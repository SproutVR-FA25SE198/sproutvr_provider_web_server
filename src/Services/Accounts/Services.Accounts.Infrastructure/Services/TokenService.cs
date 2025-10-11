using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Accounts.Application.Abstractions.Services;

namespace Services.Accounts.Infrastructure.Services;
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create a symmetric security key using the secret key from the configuration.

        #pragma warning disable CS8604 // Possible null reference argument.
        var authSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
        var credentials = new SigningCredentials
                          (authSigningKey, SecurityAlgorithms.HmacSha256);

        List<string> audiences = _configuration.GetSection("JWT:Audiences").Get<List<string>>();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _configuration["JWT:Issuer"],
            Expires = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpirationInMinutes")),
            SigningCredentials = credentials
        };
        #pragma warning restore CS8604 // Possible null reference argument.


        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (string aud in audiences)
        {
            tokenDescriptor.Audiences.Add(aud);
        }
        #pragma warning restore CS8602 // Dereference of a possibly null reference.

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];

        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CA5404 // Do not disable token validation checks
        List<string> audiences = _configuration.GetSection("JWT:Audiences").Get<List<string>>();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudiences = audiences,
            ValidIssuer = _configuration["JWT:Issuer"],
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey
                  (Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]))
        };
#pragma warning restore CA5404 // Do not disable token validation checks
#pragma warning restore CS8604 // Possible null reference argument.

        var tokenHandler = new JwtSecurityTokenHandler();

        // Validate the token and extract the claims principal and the security token.
        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        // Ensure the token is a valid JWT and uses the HmacSha256 signing algorithm.
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals
        (SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}

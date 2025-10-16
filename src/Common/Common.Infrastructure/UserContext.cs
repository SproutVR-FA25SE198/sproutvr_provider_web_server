using System.Security.Claims;
using Common.Application.Abstractions;
using Common.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Services.Accounts.Infrastructure.Services;
public class UserContext : IUserContext
{
    private static readonly AsyncLocal<CurrentUser?> _currentUser = new();

    public CurrentUser? GetCurrentUser() => _currentUser.Value;

    public void SetUser(ClaimsPrincipal user)
    {
        string? userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? email = user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                        ?? user.FindFirst(ClaimTypes.Email)?.Value;
        IEnumerable<string> roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

        _currentUser.Value = new CurrentUser(userId!, email!, roles);
    }
}


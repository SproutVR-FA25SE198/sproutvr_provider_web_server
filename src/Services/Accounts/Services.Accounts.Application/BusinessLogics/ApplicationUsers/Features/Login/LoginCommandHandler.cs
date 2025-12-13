using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Services.Accounts.Application.Abstractions.Services;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginCommandHandler(ITokenService tokenService, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        // Check if account is active
        if (user.Status == AccountStatus.Inactive)
        {
            throw new UnauthorizedAccessException("Account has been deactivated");
        }
        
        bool isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException();
        }

        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        // creating the necessary claims
        List<Claim> authClaims = [
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            ..userRoles.Select(r => new Claim(ClaimTypes.Role, r))
        ];

        if (user is Organization org)
        {
            authClaims.Add(new(JwtRegisteredClaimNames.Name, org.Name));
        }
        else if (user is SystemAdmin sysAdmin)
        {
            authClaims.Add(new(JwtRegisteredClaimNames.Name, sysAdmin.FullName));
        }

        // generating access token
        string token = _tokenService.GenerateAccessToken(authClaims);

        string refreshToken = _tokenService.GenerateRefreshToken();

        // handle saving refresh token to db later

        var result = new LoginResponseDto()
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
        return result;
    }
}


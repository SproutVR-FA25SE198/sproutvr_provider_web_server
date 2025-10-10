using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

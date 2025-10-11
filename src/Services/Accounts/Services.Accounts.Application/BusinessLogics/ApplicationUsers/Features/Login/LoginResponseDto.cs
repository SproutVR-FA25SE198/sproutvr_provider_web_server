using System.ComponentModel.DataAnnotations;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.Login;
public class LoginResponseDto
{
    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }

}

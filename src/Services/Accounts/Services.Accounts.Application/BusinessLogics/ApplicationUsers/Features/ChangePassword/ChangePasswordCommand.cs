using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ChangePassword;
public class ChangePasswordCommand : IRequest<IdentityResult>
{
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string CurrentPassword { get; set; }
}

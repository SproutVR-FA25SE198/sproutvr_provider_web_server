using Common.Application.Abstractions;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ChangePassword;
public class ChangePasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserContext userContext) : IRequestHandler<ChangePasswordCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new OperationFailedException("New password and confirmation do not match.");
        }
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();
        ApplicationUser? user = await userManager.FindByEmailAsync(userClaims!.Email!);

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), user!.Email!);
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result; 

    }
}

using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Old password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from old password");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirmation password is required.")
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirmation password do not match.");
    }
}


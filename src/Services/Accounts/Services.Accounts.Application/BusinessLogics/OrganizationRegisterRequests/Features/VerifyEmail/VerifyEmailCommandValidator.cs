using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.VerifyEmail;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.OrganizationRegisterRequestId)
            .NotEmpty().WithMessage("Organization register request ID is required");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}


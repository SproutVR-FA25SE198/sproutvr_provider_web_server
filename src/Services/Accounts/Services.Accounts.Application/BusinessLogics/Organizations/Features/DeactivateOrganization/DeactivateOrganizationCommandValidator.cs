using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.DeactivateOrganization;

public class DeactivateOrganizationCommandValidator : AbstractValidator<DeactivateOrganizationCommand>
{
    public DeactivateOrganizationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Organization ID is required");
    }
}


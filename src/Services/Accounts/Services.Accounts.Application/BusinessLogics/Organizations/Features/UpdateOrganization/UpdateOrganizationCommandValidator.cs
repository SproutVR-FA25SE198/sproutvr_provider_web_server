using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Organization ID is required");

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        });

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters");
        });

        When(x => x.PhoneNumber != null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9]{10,11}$").WithMessage("Phone number must be 10 or 11 digits");
        });

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email is not valid");
        });
    }
}


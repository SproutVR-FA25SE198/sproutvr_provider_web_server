using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;

public class CreateOrganizationRequestCommandValidator : AbstractValidator<CreateOrganizationRequestCommand>
{
    public CreateOrganizationRequestCommandValidator()
    {
        RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("Organization name is required")
            .MaximumLength(200).WithMessage("Organization name must not exceed 200 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Contact phone is required")
            .Matches(@"^[0-9]{10,11}$").WithMessage("Contact phone must be 10 or 11 digits");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required")
            .EmailAddress().WithMessage("Contact email is not valid");
    }
}


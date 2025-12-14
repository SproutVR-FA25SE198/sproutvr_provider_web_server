using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganizationProfile;

public class UpdateOrganizationProfileCommandValidator : AbstractValidator<UpdateOrganizationProfileCommand>
{
    public UpdateOrganizationProfileCommandValidator()
    {
        RuleFor(x => x.MACAdress)
            .NotEmpty().WithMessage("MAC Address is required")
            .Matches(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$")
            .WithMessage("MAC Address format is invalid");
    }
}


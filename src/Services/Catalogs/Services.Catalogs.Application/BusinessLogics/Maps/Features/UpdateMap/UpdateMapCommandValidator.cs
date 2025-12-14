using FluentValidation;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.UpdateMap;

public sealed class UpdateMapCommandValidator : AbstractValidator<UpdateMapCommand>
{
    public UpdateMapCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Map ID is required");

        RuleFor(x => x.Dto)
            .NotNull().WithMessage("Update data is required")
            .SetValidator(new UpdateMapDtoValidator());
    }
}

public class UpdateMapDtoValidator : AbstractValidator<UpdateMapDto>
{
    public UpdateMapDtoValidator()
    {
        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");
        });

        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Status), () =>
        {
            RuleFor(x => x.Status)
                .Must(status => Enum.TryParse<MapStatus>(status, out _))
                .WithMessage("Status must be a valid map status");
        });
    }
}


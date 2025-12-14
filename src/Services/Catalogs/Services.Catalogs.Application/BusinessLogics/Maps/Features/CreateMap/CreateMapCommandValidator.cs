using FluentValidation;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;

public sealed class CreateMapCommandValidator : AbstractValidator<CreateMapCommand>
{
    public CreateMapCommandValidator()
    {
        RuleFor(x => x.MapDataFile)
            .NotNull().WithMessage("Map data file is required")
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Map data file cannot be empty")
            .Must(file => file == null || file.FileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase))
            .WithMessage("Map data file must be a ZIP file");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Subject ID is required");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .Must(price => decimal.TryParse(price, out decimal result) && result >= 0)
            .WithMessage("Price must be a valid non-negative number");
    }
}

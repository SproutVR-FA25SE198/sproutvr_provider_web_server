using FluentValidation;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.DeleteMap;

public sealed class DeleteMapCommandValidator : AbstractValidator<DeleteMapCommand>
{
    public DeleteMapCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Map ID is required");
    }
}


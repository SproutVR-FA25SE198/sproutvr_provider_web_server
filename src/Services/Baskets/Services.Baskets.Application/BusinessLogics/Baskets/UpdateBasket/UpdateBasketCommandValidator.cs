using FluentValidation;

namespace Services.Baskets.Application.BusinessLogics.Baskets.UpdateBasket;

public class UpdateBasketCommandValidator : AbstractValidator<UpdateBasketCommand>
{
    public UpdateBasketCommandValidator()
    {
        RuleFor(x => x.Basket)
            .NotNull().WithMessage("Basket is required");

        RuleFor(x => x.Basket.Id)
            .NotEmpty().WithMessage("Basket ID is required")
            .When(x => x.Basket != null);
    }
}


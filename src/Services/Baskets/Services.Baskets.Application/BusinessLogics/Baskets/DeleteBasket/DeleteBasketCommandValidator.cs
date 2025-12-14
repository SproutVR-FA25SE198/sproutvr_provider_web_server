using FluentValidation;

namespace Services.Baskets.Application.BusinessLogics.Baskets.DeleteBasket;

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Basket ID is required");
    }
}


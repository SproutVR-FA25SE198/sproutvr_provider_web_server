using FluentValidation;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CreateOrderDto)
            .NotNull().WithMessage("Order data is required")
            .SetValidator(new CreateOrderDtoValidator());
    }
}

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.RepresentativeName)
            .NotEmpty().WithMessage("Representative name is required")
            .MaximumLength(200).WithMessage("Representative name must not exceed 200 characters");

        RuleFor(x => x.RepresentativePhone)
            .NotEmpty().WithMessage("Representative phone is required")
            .Matches(@"^[0-9]{10,11}$").WithMessage("Representative phone must be 10 or 11 digits");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required");

        RuleFor(x => x.Basket)
            .NotNull().WithMessage("Basket is required");

        RuleFor(x => x.Basket.BasketItems)
            .NotEmpty().WithMessage("Basket must contain at least one item")
            .When(x => x.Basket != null);
    }
}


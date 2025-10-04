using MediatR;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.UpdateBasket;
public class UpdateBasketCommand : IRequest<Basket>
{
    public Basket Basket { get; }

    public UpdateBasketCommand(Basket basket)
    {
        Basket = basket;
    }
}

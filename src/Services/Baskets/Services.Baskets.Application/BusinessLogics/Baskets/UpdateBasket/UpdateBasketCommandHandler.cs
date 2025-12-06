using Common.Domain.Exceptions;
using MediatR;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.UpdateBasket;
public class UpdateBasketCommandHandler(IBasketRepository repo) : IRequestHandler<UpdateBasketCommand, Basket>
{
    public async Task<Basket> Handle(UpdateBasketCommand request, CancellationToken cancellationToken)
    {
        Basket updatedBasket = await repo.UpdateBasketAsync(request.Basket)
            ?? throw new NotFoundException(nameof(Basket), request.Basket.Id);

        return updatedBasket;
    }
}

using MediatR;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.GetBasketById;
public class GetBasketByIdQueryHandler(IBasketRepository repo) : IRequestHandler<GetBasketByIdQuery, Basket>
{
    public async Task<Basket> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
    {
        Basket basket = await repo.GetBasketByIdAsync(request.Id);

        return basket ?? new Basket() { Id = request.Id };
    }
}

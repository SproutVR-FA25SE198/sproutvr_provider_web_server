using MediatR;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.GetBasketById;
public class GetBasketByIdQuery : IRequest<Basket>
{
    public string Id { get; }
    public GetBasketByIdQuery(string id)
    {
        Id = id;
    }
}
